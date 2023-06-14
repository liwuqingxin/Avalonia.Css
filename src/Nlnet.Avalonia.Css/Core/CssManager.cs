using Avalonia;

namespace Nlnet.Avalonia.Css
{
    public interface ICssManager
    {
        public string? Theme { get; set; }

        public string? Mode { get; set; }
    }

    public class CssManager : AvaloniaObject, ICssManager
    {
        public string? Theme
        {
            get { return GetValue(ThemeProperty); }
            set { SetValue(ThemeProperty, value); }
        }
        public static readonly StyledProperty<string?> ThemeProperty = AvaloniaProperty
            .Register<CssManager, string?>(nameof(Theme), "light");

        public string? Mode
        {
            get { return GetValue(ModeProperty); }
            set { SetValue(ModeProperty, value); }
        }
        public static readonly StyledProperty<string?> ModeProperty = AvaloniaProperty
            .Register<CssManager, string?>(nameof(Mode), "");
    }
}
