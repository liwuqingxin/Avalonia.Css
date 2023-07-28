using Avalonia;

namespace Nlnet.Avalonia.Css;

public static class CssExtension
{
    #region CssBuilder & TypeResolver

    /// <summary>
    /// Use the default <see cref="ICssBuilder"/> as avalonia css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssDefaultBuilder(this AppBuilder builder)
    {
        CssBuilder.UseDefaultBuilder();
        builder.WriteLine($"==== Avalonia css default builder used...");
        return builder;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the default <see cref="ICssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="typeResolver"></param>
    /// <returns></returns>
    public static AppBuilder WithTypeResolverForDefaultBuilder(this AppBuilder builder, ITypeResolver typeResolver)
    {
        CssBuilder.Default.LoadResolver(typeResolver);
        return builder;
    }

    #endregion



    #region Behavior

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

    #endregion
}