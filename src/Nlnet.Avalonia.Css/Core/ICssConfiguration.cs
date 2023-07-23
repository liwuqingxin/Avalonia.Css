using Avalonia.Styling;

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
}
