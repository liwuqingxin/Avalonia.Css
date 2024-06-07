using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Nlnet.Avalonia.Controls;

namespace Nlnet.Avalonia;

public interface IMagicLayout
{
    public IEnumerable<string> GetNames();
        
    public Size MeasureOverride(MagicPanel panel, IReadOnlyList<Control> children, Size availableSize);

    public Size ArrangeOverride(MagicPanel panel, IReadOnlyList<Control> children, Size finalSize);

    IInputElement? GetNavigatedControl(MagicPanel panel, NavigationDirection direction, IInputElement? from, bool wrap);
    
    void ApplySetter(MagicPanel panel, string property, string value);
}