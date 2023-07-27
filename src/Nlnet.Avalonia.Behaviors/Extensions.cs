using Avalonia;
using Avalonia.Metadata;
using Nlnet.Avalonia.Css;

namespace Nlnet.Avalonia.Behaviors;

public static class Extensions
{
    private static ITypeResolver GetInternalTypeResolver()
    {
        var typeResolver = new GenericResolver<Acss>();
        typeResolver.TryAddType("acss", typeof(Acss));
        return typeResolver;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the default <see cref="ICssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder WithAcssBehaviorTypeResolverForDefaultBuilder(this AppBuilder builder)
    {
        CssBuilder.Default.LoadResolver(GetInternalTypeResolver());
        return builder;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the <see cref="ICssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cssBuilder"></param>
    /// <returns></returns>
    public static AppBuilder WithAcssBehaviorTypeResolver(this AppBuilder builder, ICssBuilder cssBuilder)
    {
        cssBuilder.LoadResolver(GetInternalTypeResolver());
        return builder;
    }
}