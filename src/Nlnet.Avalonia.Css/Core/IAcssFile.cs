using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Represents an acss file instance, which is also a <see cref="IStyle"/> object.
/// </summary>
public interface IAcssFile : IStyle
{
    /// <summary>
    /// The <see cref="ISource"/>.
    /// </summary>
    public ISource Source { get; }

    /// <summary>
    /// Reload this from the file.
    /// </summary>
    /// <param name="reapplyStyle"></param>
    public void Reload(bool reapplyStyle);

    /// <summary>
    /// Unload this file.
    /// </summary>
    public void Unload();
}
