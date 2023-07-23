using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Represents a acss file instance, which is also a <see cref="IStyle"/> object.
/// </summary>
public interface ICssFile : IStyle
{
    /// <summary>
    /// The standard file path.
    /// </summary>
    public string StandardFilePath { get; }

    /// <summary>
    /// Reload this from the file.
    /// </summary>
    /// <param name="reapplyStyle"></param>
    public void Reload(bool reapplyStyle);

    /// <summary>
    /// Unload this file.
    /// </summary>
    public void Unload(bool reapplyStyle);
}
