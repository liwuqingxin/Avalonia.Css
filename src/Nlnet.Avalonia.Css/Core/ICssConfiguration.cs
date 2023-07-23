namespace Nlnet.Avalonia.Css;

/// <summary>
/// Global css configuration.
/// </summary>
public interface ICssConfiguration
{
    /// <summary>
    /// The theme that decides which accent color would be used.
    /// </summary>
    public string? Theme { get; set; }

    /// <summary>
    /// The mode that include dark mode or light mode.
    /// </summary>
    public string? Mode { get; set; }
}
