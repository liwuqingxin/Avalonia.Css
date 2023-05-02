using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    public interface ITypeProvider
    {
        public bool TryGetType(string name, out Type? type);
    }

    public class AvaloniaControlsTypeProvider : ITypeProvider
    {
        private readonly Dictionary<string, Type> _types;

        public AvaloniaControlsTypeProvider()
        {
            var typeSink = typeof(Control);
            var assembly = typeSink.Assembly;
            var types    = assembly.GetTypes().Where(t => t.IsAssignableTo(typeof(IVisual)));

            _types = types.ToDictionary(type => type.Name, type => type);
        }

        public bool TryGetType(string name, out Type? type)
        {
            return _types.TryGetValue(name, out type);
        }
    }

    public class CssTypeProviderManager
    {
        public static CssTypeProviderManager Instance { get; } = new();

        private CssTypeProviderManager()
        {
            
        }

        private readonly List<ITypeProvider> _providers = new()
        {
            new AvaloniaControlsTypeProvider(),
        };

        public void LoadProvider(ITypeProvider provider)
        {
            _providers.Add(provider);
        }

        public void UnloadProvider(ITypeProvider provider)
        {
            _providers.Remove(provider);
        }

        public bool TryGetType(string name, out Type? type)
        {
            foreach (var provider in _providers)
            {
                if (provider.TryGetType(name, out type))
                {
                    return true;
                }
            }

            type = null;
            return false;
        }
    }
}
