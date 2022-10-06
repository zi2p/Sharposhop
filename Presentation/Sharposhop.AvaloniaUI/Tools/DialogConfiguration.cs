using System.Collections.Generic;
using Avalonia.Controls;

namespace Sharposhop.AvaloniaUI.Tools;

public record DialogConfiguration(
    IReadOnlyCollection<FileDialogFilter> OpenFilters,
    IReadOnlyCollection<FileDialogFilter> SaveFilters);