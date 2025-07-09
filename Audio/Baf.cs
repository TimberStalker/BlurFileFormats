using BlurFileFormats.Audio.BafAudioFormat.Entities;
using BlurFileFormats.FlaskReflection.Entities;
using BlurFileFormats.SerializationFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    public static Baf Parse(string filepath)
    {
        using var fileStream = File.OpenRead(filepath);
        return Parse(fileStream);
    }
    public static Baf Parse(Stream source)
    {
        var bafEntity = (BafEntity)FlaskSerializer.Read(source);

        Baf baf = new Baf(bafEntity.Header.Name);

        foreach (var (wavel, data) in bafEntity.Wavels.Zip(bafEntity.Data))
        {
            baf.Tracks.Add(new BafTrack
            {
                Name = wavel.Name,
                SampleCount = wavel.SampleCount,
                SampleRate = wavel.SampleRate,
                ChannelCount = wavel.ChannelCount,
                TrackCount = wavel.Tracks,
                Looping = wavel.Loops,
                AudioStream = data.Data
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

    public required byte[] AudioStream { get; set; }

    public bool Looping { get; set; }
}
