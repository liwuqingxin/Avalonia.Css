using Avalonia.Controls;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

public interface ITypeResolverManager : IResolverManager<ITypeResolver>
{

}

internal class TypeResolverManager : ResolverManager<ITypeResolver>, ITypeResolverManager
{
    public TypeResolverManager()
    {
        // Avalonia.Controls
        LoadResolver(new GenericTypeResolver<Control>());
        // Avalonia.Base
        LoadResolver(new GenericTypeResolver<Transform>());
        // Internal Resolver.
        LoadResolver(new InternalTypeResolver());
    }
}