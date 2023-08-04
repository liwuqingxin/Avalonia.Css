using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Acss behavior base class that provides the abstraction of attaching and detaching.
/// </summary>
public abstract class AcssBehavior
{
    protected AvaloniaObject? AssociatedObject { get; private set; }

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

/// <summary>
/// A generic derived class from <see cref="AcssBehavior{T}"/>, which provides the <see cref="Get"/> method additionally.
/// </summary>
/// <typeparam name="T"></typeparam>
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
