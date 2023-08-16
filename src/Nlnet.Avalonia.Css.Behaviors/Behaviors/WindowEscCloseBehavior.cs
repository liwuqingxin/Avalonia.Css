using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace Nlnet.Avalonia.Css.Behaviors;

[Behavior("window.esc.close", typeof(Acss))]
public class WindowEscCloseBehavior: AcssBehavior<WindowEscCloseBehavior>
{
    protected override void OnAttached(AvaloniaObject target)
    {
        if (target is not Window window)
        {
            return;
        }

        window.KeyDown -= WindowOnKeyDown;
        window.KeyDown += WindowOnKeyDown;
    }
    
    protected override void OnDetached(AvaloniaObject target)
    {
        if (target is not Window window)
        {
            return;
        }

        window.KeyDown -= WindowOnKeyDown;
    }
    
    private static void WindowOnKeyDown(object? sender, KeyEventArgs e)
    {
        if (sender is not Window window)
        {
            return;
        }
        if (e.Key == Key.Escape)
        {
            window.Close();
        }
    }
}