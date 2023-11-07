namespace Nlnet.Avalonia.Css;

public interface IAcssContext : IServiceProvider
{
    /// <summary>
    /// Try to add an acss file to the context.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryAddAcssFile(IAcssFile file);

    /// <summary>
    /// Try to remove an acss file from the context.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryRemoveAcssFile(IAcssFile file);

    /// <summary>
    /// Try to get an acss file from the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="file"></param>
    /// <returns></returns>
    internal bool TryGetAcssFile(ISource source, out IAcssFile? file);

    /// <summary>
    /// Try to add an acss tokens to the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryAddAcssTokens(ISource source, AcssTokens tokens);

    /// <summary>
    /// Try to remove an acss tokens to the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryRemoveAcssTokens(ISource source, AcssTokens tokens);

    /// <summary>
    /// Try to get an acss tokens from the context.
    /// </summary>
    /// <param name="source"></param>
    /// <param name="tokens"></param>
    /// <returns></returns>
    internal bool TryGetAcssTokens(ISource source, out AcssTokens? tokens);

    /// <summary>
    /// Enable or disable transitions for the context.
    /// </summary>
    /// <param name="enable"></param>
    public void EnableTransitions(bool enable);

    /// <summary>
    /// Reload whole context.
    /// </summary>
    public void Reload();
}