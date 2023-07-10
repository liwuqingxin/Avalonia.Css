using System;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

public static class CssExtension
{
    /// <summary>
    /// Use the default <see cref="ICssBuilder"/> as avalonia css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T UseAvaloniaCssDefaultBuilder<T>(this T builder) where T : AppBuilderBase<T>, new()
    {
        CssBuilder.UseDefaultBuilder();
        return builder;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the default <see cref="ICssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="typeResolver"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    [Obsolete("It makes UI choppy in avalonia preview-4.")]
    public static T WithTypeResolverForDefaultBuilder<T>(this T builder, ITypeResolver typeResolver) where T : AppBuilderBase<T>, new()
    {
        CssBuilder.Default.LoadResolver(typeResolver);
        return builder;
    }
}