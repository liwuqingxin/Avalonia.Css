using System;
using Avalonia;

namespace Nlnet.Avalonia.Behaviors;

public abstract class AcssBehavior
{
    internal AvaloniaObject? AssociatedObject { get; private set; }

    internal Type? TargetType { get; set; }

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

    protected abstract AcssBehavior Get();

    internal abstract void DoNotDeriveThisUseGenericInstead();

    protected T? As<T>() where T : AvaloniaObject
    {
        return AssociatedObject as T;
    }
}

public abstract class AcssBehavior<T> : AcssBehavior where T : AcssBehavior<T>, new()
{
    protected override T Get()
    {
        return new T();
    }

    internal override void DoNotDeriveThisUseGenericInstead()
    {
        
    }
}
