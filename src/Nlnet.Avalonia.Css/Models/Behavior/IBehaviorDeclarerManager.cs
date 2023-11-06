using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Define a collection of acss behaviors.
/// </summary>
public interface IBehaviorDeclarer
{

}

/// <summary>
/// Management for all registered <see cref="IBehaviorDeclarer"/>.
/// </summary>
public interface IBehaviorDeclarerManager : IService
{
    public void RegisterDeclarer<T>(string? key) where T : IBehaviorDeclarer;

    public void UnregisterDeclarer(string? key);
    
    public bool TryGetBehaviorDeclarer(string? key, out Type? declarerType);
}

internal class BehaviorDeclarerManager : IBehaviorDeclarerManager
{
    private readonly Dictionary<string, Type> _behaviorDeclarers = new();

    public void Initialize()
    {

    }

    public void RegisterDeclarer<T>(string? key) where T : IBehaviorDeclarer
    {
        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        lock (this)
        {
            if (_behaviorDeclarers.TryGetValue(key, out var v))
            {
                throw new InvalidOperationException($"The key '{key}' existed for type '{v}'.");
            }

            _behaviorDeclarers[key] = typeof(T);
        }
    }

    public void UnregisterDeclarer(string? key)
    {
        if (key == null)
        {
            return;
        }
        lock (this)
        {
            _behaviorDeclarers.Remove(key);
        }
    }

    public bool TryGetBehaviorDeclarer(string? key, out Type? declarerType)
    {
        if (key == null)
        {
            declarerType = null;
            return false;
        }
        lock (this)
        {
            if (_behaviorDeclarers.TryGetValue(key, out var t))
            {
                declarerType = t;
                return true;
            }
        }

        declarerType = null;
        return false;
    }
}