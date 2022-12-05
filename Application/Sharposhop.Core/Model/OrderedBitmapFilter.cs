using Sharposhop.Core.BitmapImages.Filtering;

namespace Sharposhop.Core.Model;

public readonly record struct OrderedBitmapFilter(int Order, IBitmapFilter Filter);