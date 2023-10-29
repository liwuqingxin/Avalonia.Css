using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    // TODO Dynamic configurations and filters.
    /// <summary>
    /// Default implementation for <see cref="IAcssConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// Could not derived from <see cref="AvaloniaObject"/>, which will cause ui choppy.
    /// </remarks>
    internal class AcssConfiguration : IAcssConfiguration
    {
        public string? Accent { get; set; }

        public bool EnableTransitions { get; set; } = true;

        public void Initialize()
        {
            
        }
    }
}
