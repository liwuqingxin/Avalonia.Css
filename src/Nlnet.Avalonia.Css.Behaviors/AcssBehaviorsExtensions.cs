using Avalonia;
using DynamicData.Kernel;

namespace Nlnet.Avalonia.Css.Behaviors;

public static class AcssBehaviorsExtensions
{
    #region Behavior

    /// <summary>
    /// Use acss behavior feature for default css builder.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder UseAcssBehaviorForDefaultBuilder(this AppBuilder builder)
    {
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
        cssBuilder.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<Acss>());
        cssBuilder.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss).ToLower());
        cssBuilder.BehaviorDeclarerManager.RegisterDeclarer<Acss>(nameof(Acss));

        return builder;
    }

    #endregion
}