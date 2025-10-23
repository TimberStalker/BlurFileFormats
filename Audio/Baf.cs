using BlurFileFormats.Audio.BafAudioFormat.Entities;
using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.SerializationFramework;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BlurFileFormats.Audio;
public class Baf
{
    static DataSerializer FlaskSerializer { get; } = DataSerializer.Create(typeof(BafEntity));
    public string Name { get; set; }
    public List<BafTrack> Tracks { get; } = [];
    public Baf(string name)
    {
        Name = name;
    }
    static float[][] ps_adpcm_coefs_f = [
        [ 0.0f      ,  0.0f       ], //[   0.0        ,   0.0        ],
        [ 0.9375f   ,  0.0f       ], //[  60.0 / 64.0 ,   0.0        ],
        [ 1.796875f , -0.8125f    ], //[ 115.0 / 64.0 , -52.0 / 64.0 ],
        [ 1.53125f  , -0.859375f  ], //[  98.0 / 64.0 , -55.0 / 64.0 ],
        [ 1.90625f  , -0.9375f    ], //[ 122.0 / 64.0 , -60.0 / 64.0 ],
        /* extended table used in few PS3 games, found in ELFs */
        [ 0.46875f  , -0.0f       ], //[  30.0 / 64.0 ,  -0.0 / 64.0 ],
        [ 0.8984375f, -0.40625f   ], //[  57.5 / 64.0 , -26.0 / 64.0 ],
        [ 0.765625f , -0.4296875f ], //[  49.0 / 64.0 , -27.5 / 64.0 ],
        [ 0.953125f , -0.46875f   ], //[  61.0 / 64.0 , -30.0 / 64.0 ],
        [ 0.234375f , -0.0f       ], //[  15.0 / 64.0 ,  -0.0 / 64.0 ],
        [ 0.44921875f,-0.203125f  ], //[  28.75/ 64.0 , -13.0 / 64.0 ],
        [ 0.3828125f, -0.21484375f], //[  24.5 / 64.0 , -13.75/ 64.0 ],
        [ 0.4765625f, -0.234375f  ], //[  30.5 / 64.0 , -15.0 / 64.0 ],
        [ 0.5f      , -0.9375f    ], //[  32.0 / 64.0 , -60.0 / 64.0 ],
        [ 0.234375f , -0.9375f    ], //[  15.0 / 64.0 , -60.0 / 64.0 ],
        [ 0.109375f , -0.9375f    ], //[   7.0 / 64.0 , -60.0 / 64.0 ],
];
    static int[][] ps_adpcm_coefs_i = [
        [   0 ,   0 ],
        [  60 ,   0 ],
        [ 115 , -52 ],
        [  98 , -55 ],
        [ 122 , -60 ]
    ];
    public static Baf Parse(string filepath)
    {
        using var fileStream = File.OpenRead(filepath);
        return Parse(fileStream);
    }
    readonly struct FrameSample
    {
        public readonly int coefIndex;
        public readonly int shiftFactor;
        public readonly int sample;

        public FrameSample(int coefIndex, int shiftFactor, int sample)
        {
            this.coefIndex = coefIndex;
            this.shiftFactor = shiftFactor;
            this.sample = sample;
        }
    }
    static IEnumerable<FrameSample> GetFrameSamples(byte[] bytes, int frameSize)
    {
        for(int i = 0; i < bytes.Length; i += frameSize)
        {
            int coefIndex = bytes[i] >> 4 & 0xf;
            int shiftFactor = bytes[i] & 0xf;
            for(int j = 1; j < frameSize; j++)
            {
                int nextByte = bytes[i+j];
                yield return new FrameSample(coefIndex, shiftFactor, nextByte & 0xf);
                yield return new FrameSample(coefIndex, shiftFactor, (nextByte >> 4) & 0xf);
            }
        }
    }
    public static Baf Parse(Stream source)
    {
        var bafEntity = (BafEntity)FlaskSerializer.Read(source);

        Baf baf = new Baf(bafEntity.Header.Name);


        int hist1 = 0;
        int hist2 = 0;
        foreach (var (wavel, data) in bafEntity.Wavels.Zip(bafEntity.Data))
        {
            const int frameSize = 0x21;
            float[] audioStream = new float[wavel.SampleCount * wavel.ChannelCount];

            var samples = GetFrameSamples(data.Bytes, frameSize);

            int sampleIndex = 0;
            int channelIndex = 0;
            foreach (var sample in samples)
            {
                int sampleNibble = sample.sample;
                sampleNibble &= 0xf;

                sampleNibble <<= 12;
                sampleNibble &= 0xf000;
                sampleNibble >>= sample.shiftFactor;

                int final = (int)(sampleNibble + ps_adpcm_coefs_f[sample.coefIndex][0] * hist1 + ps_adpcm_coefs_f[sample.coefIndex][1] * hist2);
                //int final = sampleNibble + ((ps_adpcm_coefs_i[sample.coefIndex][0] * hist1 + ps_adpcm_coefs_i[sample.coefIndex][1] * hist2) >> 6);
                final = Math.Clamp(final, short.MinValue, short.MaxValue);
                audioStream[channelIndex + sampleIndex * wavel.ChannelCount] = final;

                hist2 = hist1;
                hist1 = final;

                sampleIndex++;
                if(sampleIndex >= wavel.SampleCount)
                {
                    sampleIndex = 0;
                    channelIndex++;
                    if (channelIndex >= wavel.ChannelCount)
                        break;
                }
            }

            baf.Tracks.Add(new BafTrack
            {
                Name = wavel.Name,
                SampleCount = wavel.SampleCount,
                SampleRate = wavel.SampleRate,
                ChannelCount = wavel.ChannelCount,
                TrackCount = wavel.Tracks,
                Looping = wavel.Loops,
                AudioStream = audioStream
            });
        }

        return baf;
    }
}

public class BafTrack
{
    public required string Name { get; set; }
    public required uint SampleCount{ get; set; }
    public required uint SampleRate { get; set; }

    public required int ChannelCount { get; set; }
    public required int TrackCount { get; set; }

    public required float[] AudioStream { get; set; }

    public bool Looping { get; set; }
}
