using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Controls;

public interface ILayoutDefinition
{
    public string GetName();
        
    public Size MeasureOverride(Size availableSize, IReadOnlyList<Control> children);

    public Size ArrangeOverride(Size finalSize, IReadOnlyList<Control> children);
}