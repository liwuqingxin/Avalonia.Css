namespace Nlnet.Avalonia.Css;

/// <summary>
/// Define a prefer source owner that can use the prefer source if it is available.
/// </summary>
public interface IPreferSourceOwner
{
    /// <summary>
    /// The prefer source.
    /// </summary>
    public ISource? PreferSource { get; set; }

    /// <summary>
    /// Use the recommended prefer source.
    /// </summary>
    public void UseRecommendedPreferSource();
}