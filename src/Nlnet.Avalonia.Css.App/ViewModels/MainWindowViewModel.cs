using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using DynamicData;
using Nlnet.Avalonia.Css.Fluent;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private string? _mode = "light";
        private string? _theme = "blue";
        private bool _isLoading = true;

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

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                if (value == _isLoading)
                    return;
                _isLoading = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<GalleryItem>? GalleryItems { get; set; }

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

            GalleryItems = new ObservableCollection<GalleryItem>();

            LoadService.GetGalleryItemAsync(typeof(MainWindowViewModel)).ContinueWith(t =>
            {
                foreach (var item in t.Result)
                {
                    item.Icon = $"avares://Nlnet.Avalonia.Css.App/Assets/Svg/{item.ViewType.Name[..^4]}.svg";
                }
                GalleryItems.AddRange(t.Result);
            });
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName is nameof(Theme))
            {
                CssBuilder.Default.Configuration.Theme = Theme;

                var cssTheme = Application.Current?.Styles.FirstOrDefault(s => s is CssFluentTheme) as CssFluentTheme;
                cssTheme?.UpdateTheme();
                cssTheme?.UpdateResource();
            }
            else if (propertyName is nameof(Mode))
            {
                CssBuilder.Default.Configuration.Mode = Mode;

                var cssTheme = Application.Current?.Styles.FirstOrDefault(s => s is CssFluentTheme) as CssFluentTheme;
                cssTheme?.UpdateMode();
            }
        }
    }
}
