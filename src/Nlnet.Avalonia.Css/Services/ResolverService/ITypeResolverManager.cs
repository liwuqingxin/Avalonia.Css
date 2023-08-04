using Avalonia.Controls;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// For type resolving.
/// </summary>
public interface ITypeResolverManager : IResolverManager<ITypeResolver>
{

}

/// <summary>
/// The default implementation for <see cref="ITypeResolverManager"/>.
/// By default, the resolver for 'Avalonia.Controls', 'Avalonia.Base' are loaded.
/// </summary>
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