using Avalonia;

namespace Nlnet.Avalonia.Css.Behaviors;

public static class AcssBehaviorsExtensions
{
    #region Behavior

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the default <see cref="ICssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static AppBuilder WithAcssBehaviorResolverForDefaultBuilder(this AppBuilder builder)
    {
        CssBuilder.Default.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<ComboBoxPopupAlignBehavior>());
        return builder;
    }

    /// <summary>
    /// Add a <see cref="ITypeResolver"/> to the <see cref="ICssBuilder"/>.
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="cssBuilder"></param>
    /// <returns></returns>
    public static AppBuilder WithAcssBehaviorResolver(this AppBuilder builder, ICssBuilder cssBuilder)
    {
        cssBuilder.BehaviorResolverManager.LoadResolver(new GenericBehaviorResolver<ComboBoxPopupAlignBehavior>());
        return builder;
    }

    #endregion
}