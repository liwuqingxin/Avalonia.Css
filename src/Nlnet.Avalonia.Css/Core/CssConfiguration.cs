using Avalonia;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Default implementation for <see cref="ICssConfiguration"/>.
    /// </summary>
    internal class CssConfiguration : AvaloniaObject, ICssConfiguration
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
