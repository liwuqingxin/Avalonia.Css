using Avalonia;

namespace Nlnet.Avalonia.Behaviors;

public abstract class AcssBehavior
{
    public AvaloniaObject? AssociatedObject { get; private set; }

    internal void Attach(AvaloniaObject target)
    {
        AssociatedObject = target;
        OnAttached(target);
    }

    internal void Detach(AvaloniaObject target)
    {
        OnDetached(target);
        AssociatedObject = null;
    }

    protected abstract void OnAttached(AvaloniaObject target);

    protected abstract void OnDetached(AvaloniaObject target);

    protected T? Get<T>() where T : AvaloniaObject
    {
        return AssociatedObject as T;
    }
}
