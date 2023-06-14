using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using DynamicData;
using JetBrains.Annotations;
using Nlnet.Avalonia.Css.Fluent;

namespace Nlnet.Avalonia.Css.App
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private string? _mode = "light";
        private string? _theme = "blue";

        public  List<string> Modes { get; set; }

        public List<string> Themes { get; set; }

        public string? Mode
        {
            get => _mode;
            set
            {
                if (value == _mode)
                    return;
                _mode = value;
                OnPropertyChanged();
            }
        }

        public string? Theme
        {
            get => _theme;
            set
            {
                if (value == _theme)
                    return;
                _theme = value;
                OnPropertyChanged();
            }
        }

        public MainWindowViewModel()
        {
            Modes = new List<string>()
            {
                "light",
                "dark",
            };

            Themes = new List<string>()
            {
                "blue",
                "red",
                "orange",
                "green",
                "gold",
                "lime",
                "cyan",
                "purple",
                "pink-purple",
                "magenta",
            };
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(Theme))
            {
                ServiceLocator.GetService<ICssManager>().Theme = Theme;
                
                var cssTheme = Application.Current?.Styles.FirstOrDefault(s => s is CssFluentTheme) as CssFluentTheme;
                cssTheme?.UpdateTheme();
                cssTheme?.UpdateResource();
            }
            else if (propertyName is nameof(Mode))
            {
                ServiceLocator.GetService<ICssManager>().Mode = Mode;

                var cssTheme = Application.Current?.Styles.FirstOrDefault(s => s is CssFluentTheme) as CssFluentTheme;
                cssTheme?.UpdateMode();
            }
        }
    }
}
