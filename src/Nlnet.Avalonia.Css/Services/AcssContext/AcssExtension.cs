using Avalonia;

namespace Nlnet.Avalonia.Css;

public static class AcssExtension
{
    #region AcssContext & TypeResolver

    /// <summary>
    /// Use the default <see cref="IAcssContext"/> as avalonia css context.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssDefaultContext(this AppBuilder builder)
    {
        AcssContext.UseDefaultContext();
        builder.WriteLine($"======== Avalonia css default IAcssContext used...");
        return builder;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the default <see cref="IAcssContext"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="typeResolver"></param>
    /// <returns></returns>
    public static AppBuilder WithTypeResolverForAcssDefaultContext(this AppBuilder builder, ITypeResolver typeResolver)
    {
        var manager = AcssContext.Default.GetService<ITypeResolverManager>();
        manager.LoadResolver(typeResolver);
        return builder;
    }

    #endregion
}