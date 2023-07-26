using Avalonia;

namespace Nlnet.Avalonia.Behaviors;

public abstract class AcssBehavior
{
    public abstract void OnAttached(AvaloniaObject target);

    public abstract void OnDetached(AvaloniaObject target);
}
