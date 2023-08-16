using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Global acss configuration.
/// </summary>
public interface IAcssConfiguration
{
    /// <summary>
    /// The theme that decides which accent color would be used.
    /// </summary>
    public string? Theme { get; set; }
}
