namespace Sharposhop.Core.Enumeration;

public interface IEnumerationStrategy
{
    IEnumerable<(int X, int Y)> Enumerate(int width, int height);
    
    int AsContinuousIndex(int x, int y, int width, int height);
}