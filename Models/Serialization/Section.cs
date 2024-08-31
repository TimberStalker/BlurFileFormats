namespace BlurFileFormats.Models.Serialization;
public static partial class CPModelSerializer
{
    class Section
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }
        public short Marker1 { get; set; }
        public short Marker2 { get; set; }
        public Section(string name, int start, int length, short marker1, short marker2)
        {
            Name = name;
            Start = start;
            Length = length;
            Marker1 = marker1;
            Marker2 = marker2;
        }
        public override string ToString()
        {
            return $"{Name} | {Start:X}-{Start+Length:X}({Length:X}){{{Marker1:X}:{Marker2:X}}}";
        }
    }
}
