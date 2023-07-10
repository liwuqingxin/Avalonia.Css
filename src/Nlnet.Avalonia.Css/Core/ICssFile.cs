using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Represents a acss file instance, which is also a <see cref="IStyle"/> object.
/// </summary>
public interface ICssFile : IStyle
{
    /// <summary>
    /// Reload this from the file.
    /// </summary>
    public void Reload();
}
