using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

public abstract class AcssBehavior
{
    public AvaloniaObject? AssociatedObject { get; private set; }

    internal Type? TargetType { get; set; }

    public void Attach(AvaloniaObject target)
    {
        AssociatedObject = target;
        OnAttached(target);
    }

    public void Detach(AvaloniaObject target)
    {
        OnDetached(target);
        AssociatedObject = null;
    }

    protected abstract void OnAttached(AvaloniaObject target);

    protected abstract void OnDetached(AvaloniaObject target);

    protected internal abstract AcssBehavior Get();

    internal abstract void DoNotDeriveThisUseGenericInstead();

    protected T? As<T>() where T : AvaloniaObject
    {
        return AssociatedObject as T;
    }
}

public abstract class AcssBehavior<T> : AcssBehavior where T : AcssBehavior<T>, new()
{
    protected internal override T Get()
    {
        return new T();
    }

    internal override void DoNotDeriveThisUseGenericInstead()
    {
        
    }
}
