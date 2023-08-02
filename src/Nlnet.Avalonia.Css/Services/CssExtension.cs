using Avalonia;

namespace Nlnet.Avalonia.Css;

public static class AcssExtension
{
    #region AcssBuilder & TypeResolver

    /// <summary>
    /// Use the default <see cref="IAcssBuilder"/> as avalonia css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssDefaultBuilder(this AppBuilder builder)
    {
        AcssBuilder.UseDefaultBuilder();
        builder.WriteLine($"==== Avalonia css default builder used...");
        return builder;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the default <see cref="IAcssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="typeResolver"></param>
    /// <returns></returns>
    public static AppBuilder WithTypeResolverForDefaultBuilder(this AppBuilder builder, ITypeResolver typeResolver)
    {
        AcssBuilder.Default.TypeResolver.LoadResolver(typeResolver);
        return builder;
    }

    #endregion
}