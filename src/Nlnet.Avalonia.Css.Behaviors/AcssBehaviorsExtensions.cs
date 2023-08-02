using Avalonia;
using DynamicData.Kernel;

namespace Nlnet.Avalonia.Css.Behaviors;

public static class AcssBehaviorsExtensions
{
    #region Behavior

    private static ITypeResolver GetInternalTypeResolver()
    {
        var typeResolver = new GenericTypeResolver<Acss>();
        typeResolver.TryAddType("acss", typeof(Acss));
        return typeResolver;
    }

    /// <summary>
    /// Use acss behavior feature for default css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssBehaviorForDefaultBuilder(this AppBuilder builder)
    {
        //CssBuilder.Default.TypeResolver.LoadResolver(GetInternalTypeResolver());
        AcssBuilder.Default.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<Acss>());
        AcssBuilder.Default.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss).ToLower());
        AcssBuilder.Default.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss));

        return builder;
    }

    /// <summary>
    /// Use acss behavior feature.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cssBuilder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssBehavior(this AppBuilder builder, IAcssBuilder cssBuilder)
    {
        //cssBuilder.TypeResolver.LoadResolver(GetInternalTypeResolver());
        cssBuilder.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<Acss>());
        cssBuilder.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss).ToLower());
        cssBuilder.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss));

        return builder;
    }

    #endregion
}