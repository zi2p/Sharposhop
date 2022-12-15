namespace Sharposhop.Core.Loading.Png;

public readonly struct ChunkHeader
{
    public long Position { get; }
    public int Length { get; }
    public string Name { get; }
    
    public bool IsCritical => char.IsUpper(Name[0]);
    public bool IsPublic => char.IsUpper(Name[1]);
    public bool IsSafeToCopy => char.IsUpper(Name[3]);

    public ChunkHeader(long position, int length, string name)
    {
        Position = position;
        Length = length;
        Name = name;
    }
}