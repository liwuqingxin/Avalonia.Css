using Avalonia;


namespace Nlnet.Avalonia.Css.Behaviors;

public static class AcssBehaviorsExtensions
{
    #region Behavior

    /// <summary>
    /// Use acss behavior feature for default css context.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssBehaviorForDefaultContext(this AppBuilder builder)
    {
        var resolver = AcssContext.Default.GetService<IBehaviorResolverManager>();
        var declarer = AcssContext.Default.GetService<IBehaviorDeclarerManager>();

        resolver.LoadResolver(new GenericBehaviorResolver<Acss>());
        declarer.RegisterDeclarer<Acss>(nameof(Acss).ToLower());
        declarer.RegisterDeclarer<Acss>(nameof(Acss));

        return builder;
    }

    /// <summary>
    /// Use acss behavior feature.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssBehavior(this AppBuilder builder, IAcssContext context)
    {
        var resolver = context.GetService<IBehaviorResolverManager>();
        var declarer = context.GetService<IBehaviorDeclarerManager>();

        resolver.LoadResolver(new GenericBehaviorResolver<Acss>());
        declarer.RegisterDeclarer<Acss>(nameof(Acss).ToLower());
        declarer.RegisterDeclarer<Acss>(nameof(Acss));

        return builder;
    }

    #endregion
}