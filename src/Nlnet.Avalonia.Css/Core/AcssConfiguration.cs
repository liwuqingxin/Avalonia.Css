using Avalonia;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    这里改为动态设定，而不是使用写死的配置
    /// <summary>
    /// Default implementation for <see cref="IAcssConfiguration"/>.
    /// </summary>
    /// <remarks>
    /// Could not derived from <see cref="AvaloniaObject"/>, which will cause ui choppy.
    /// </remarks>
    internal class AcssConfiguration : IAcssConfiguration
    {
        public string? Theme
        {
            get;
            set;
        }
    }
}
