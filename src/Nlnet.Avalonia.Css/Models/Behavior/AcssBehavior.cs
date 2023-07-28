using System;
using Avalonia;

namespace Nlnet.Avalonia.Css;

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

    protected internal abstract AcssBehavior Get();

    internal abstract void DoNotDeriveThisUseGenericInstead();

    protected T? As<T>() where T : AvaloniaObject
    {
        return AssociatedObject as T;
    }



    //public static bool ApplyBehaviorByKey(ICssBuilder cssBuilder, string cls, string key)
    //{
    //    if (cssBuilder.TryGetType(cls, out var t) == false)
    //    {
    //        return false;
    //    }

    //    if (AcssBehaviorFactories.TryGetBehavior(key, out var behavior) == false)
    //    {
    //        return false;
    //    }

    //    var propertyName = $"{behavior!.GetType().Name}Property";
    //    var property = cssBuilder.Interpreter.ParseAvaloniaProperty(t!, propertyName);
    //    if (property == null)
    //    {
    //        return false;
    //    }

    //    return true;
    //}
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
