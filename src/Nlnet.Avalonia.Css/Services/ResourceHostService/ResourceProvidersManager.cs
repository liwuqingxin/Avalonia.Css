using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal class ResourceProvidersManager : IResourceProvidersManager
{
    private readonly List<WeakReference<IResourceProvider>> _references = new();

    public ResourceProvidersManager()
    {
        
    }

    void IResourceProvidersManager.RegisterResourceProvider(IResourceProvider provider)
    {
        lock (this)
        {
            _references.Add(new WeakReference<IResourceProvider>(provider));
        }
    }

    void IResourceProvidersManager.UnregisterResourceProvider(IResourceProvider provider)
    {
        lock (this)
        {
            var target = _references.FirstOrDefault(r => r.TryGetTarget(out var tar) && tar == provider);
            if (target != null)
            {
                _references.Remove(target);
            }
        }
    }

    public bool TryFindResource<T>(object key, ThemeVariant mode, out T? result)
    {
        List<WeakReference<IResourceProvider>>? list = null;
        lock (this)
        {
            list = _references.ToList();
        }

        foreach (var reference in list)
        {
            if (reference.TryGetTarget(out var target))
            {
                if (target.TryGetResource(key, mode, out var res) && res is T t)
                {
                    result = t;
                    return true;
                }
            }
        }

        if (Application.Current != null)
        {
            if (Application.Current.TryFindResource(key, out var res) && res is T t)
            {
                result = t;
                return true;
            }
        }

        this.WriteError($"Can not find the resource with key of '{key}' in {nameof(ResourceProvidersManager)}.");

        result = default;
        return false;
    }
    
    public bool TryFindResource<T>(object key, out T? result)
    {
        if (Application.Current == null)
        {
            result = default;
            return false;
        }

        var mode = Application.Current.ActualThemeVariant;

        return TryFindResource(key, mode, out result);
    }
}
