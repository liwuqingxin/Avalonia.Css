using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    public interface ITypeResolver
    {
        public bool TryGetType(string name, out Type? type);
    }

    public class AvaloniaControlsTypeResolver : ITypeResolver
    {
        private readonly Dictionary<string, Type> _types;

        public AvaloniaControlsTypeResolver()
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

    public class CssTypeResolverManager
    {
        private readonly Dictionary<Type, Type> _parseTypeAdapter = new();

        public static CssTypeResolverManager Instance { get; } = new();

        private CssTypeResolverManager()
        {
            _parseTypeAdapter.Add(typeof(IBrush),           typeof(Brush));
            _parseTypeAdapter.Add(typeof(ISolidColorBrush), typeof(Brush));
            _parseTypeAdapter.Add(typeof(SolidColorBrush),  typeof(Brush));
        }

        private readonly List<ITypeResolver> _resolvers = new()
        {
            new AvaloniaControlsTypeResolver(),
        };

        public void LoadResolver(ITypeResolver resolver)
        {
            _resolvers.Add(resolver);
        }

        public void UnloadResolver(ITypeResolver resolver)
        {
            _resolvers.Remove(resolver);
        }

        public bool TryGetType(string name, out Type? type)
        {
            foreach (var resolver in _resolvers)
            {
                if (resolver.TryGetType(name, out type))
                {
                    return true;
                }
            }

            type = null;
            return false;
        }

        public void AddParseType(Type declaredType, Type parseType)
        {
            _parseTypeAdapter[declaredType] = parseType;
        }

        public bool TryGetParseType(Type declaredType, out Type? parseType)
        {
            return _parseTypeAdapter.TryGetValue(declaredType, out parseType);

        }
    }
}
