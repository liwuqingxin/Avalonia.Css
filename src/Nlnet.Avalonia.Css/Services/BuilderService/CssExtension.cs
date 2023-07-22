using System;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Css;

public static class CssExtension
{
    /// <summary>
    /// Use the default <see cref="ICssBuilder"/> as avalonia css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAvaloniaCssDefaultBuilder(this AppBuilder builder)
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
}