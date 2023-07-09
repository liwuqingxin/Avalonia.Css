using Avalonia;

namespace Nlnet.Avalonia.Css
{
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

    /// <summary>
    /// Default implementation for <see cref="ICssConfiguration"/>.
    /// </summary>
    public class CssConfiguration : AvaloniaObject, ICssConfiguration
    {
        public string? Theme
        {
            get { return GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        public static readonly StyledProperty<string?> ThemeProperty = AvaloniaProperty
            .Register<CssConfiguration, string?>(nameof(Theme), "blue");

        public string? Mode
        {
            get { return GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public static readonly StyledProperty<string?> ModeProperty = AvaloniaProperty
            .Register<CssConfiguration, string?>(nameof(Mode), "light");
    }
}
