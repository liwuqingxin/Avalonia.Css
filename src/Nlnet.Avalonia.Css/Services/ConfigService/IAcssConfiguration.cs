using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Global acss configuration.
/// </summary>
public interface IAcssConfiguration : IService
{
    /// <summary>
    /// The theme that decides which accent color would be used.
    /// </summary>
    public string? Accent { get; set; }

    /// <summary>
    /// Indicates if enable transitions.
    /// </summary>
    public bool EnableTransitions { get; set; }
}
