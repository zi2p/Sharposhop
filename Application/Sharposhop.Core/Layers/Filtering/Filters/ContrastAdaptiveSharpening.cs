// using Sharposhop.Core.Model;
//
// namespace Sharposhop.Core.BitmapImages.Filtering.Filters;
//
// public class ContrastAdaptiveSharpening : IBitmapFilter
// {
//     public string DisplayName => "Contrast Adaptive Sharpening"
//     public event Func<ValueTask>? FilterChanged;
//     public ColorTriplet ApplyAt<T>(T reader, PlaneCoordinate coordinate) where T : IBitmapFilterReader
//     {
//         
//     }
//
//     public void Accept(IBitmapFilterVisitor visitor)
//         => visitor.Visit(this);
// }