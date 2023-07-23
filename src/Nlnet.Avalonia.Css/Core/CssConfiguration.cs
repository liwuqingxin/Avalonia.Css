using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Default implementation for <see cref="ICssConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// Could not derived from <see cref="AvaloniaObject"/>, which will cause ui choppy.
    /// </remarks>
    internal class CssConfiguration : ICssConfiguration
    {
        public string? Theme
        {
            get;
            set;
        }
    }
}
