using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Styling;
using Avalonia.Threading;
using DynamicData;
using Nlnet.Avalonia.Css.App.Views;
using Nlnet.Avalonia.Css.Fluent;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App
{
    public class MainWindowViewModel : NotifyPropertyChanged
    {
        private readonly IMainViewService _viewService;
        private ThemeVariant _theme     = ThemeVariant.Light;
        private string?      _accent    = "green";
        private bool         _isLoading = true;
        private bool         _isLocalDark;
        private bool         _isEnabled = true;
        private GalleryItem? _selectedGalleryItem;
        private GalleryItem? _delaySelectedGalleryItem;

        public  List<ThemeVariant> Modes { get; set; }

        public List<string> Accents { get; set; }

        public ObservableCollection<GalleryItem>? GalleryItems { get; set; }
        
        public ThemeVariant Theme
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

        public string? Accent
        {
            get => _accent;
            set
            {
                if (value == _accent)
                    return;
                _accent = value;
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

        public bool IsLocalDark
        {
            get => _isLocalDark;
            set
            {
                if (value == _isLocalDark)
                    return;
                _isLocalDark = value;
                OnPropertyChanged();
            }
        }

        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (value == _isEnabled) return;
                _isEnabled = value;
                OnPropertyChanged();
            }
        }

        public GalleryItem? SelectedGalleryItem
        {
            get => _selectedGalleryItem;
            set
            {
                if (Equals(value, _selectedGalleryItem)) return;
                _selectedGalleryItem = value;
                OnPropertyChanged();
                
                DelaySelectedGalleryItem = null;
                IsLoading = true;
                Dispatcher.UIThread.Post(() =>
                {
                    DelaySelectedGalleryItem = value;
                    IsLoading = false;
                }, DispatcherPriority.ApplicationIdle);
            }
        }

        public GalleryItem? DelaySelectedGalleryItem
        {
            get => _delaySelectedGalleryItem;
            set
            {
                if (Equals(value, _delaySelectedGalleryItem)) return;
                _delaySelectedGalleryItem = value;
                OnPropertyChanged();
                _viewService.ScrollToHome();
            }
        }

        public MainWindowViewModel(IMainViewService viewService)
        {
            _viewService = viewService;
            Modes = new List<ThemeVariant>()
            {
                ThemeVariant.Light,
                ThemeVariant.Dark,
            };

            Accents = new List<string>()
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

            LoadService.XmlParser = new XCaseXamlParser<IndependentCase>();
            LoadService.GetGalleryItemAsync(typeof(MainWindowViewModel)).ContinueWith(t =>
            {
                foreach (var item in t.Result)
                {
                    item.Icon = $"avares://Nlnet.Avalonia.Css.App/Assets/Svg/{item.ViewType.Name[..^4]}.svg";
                    //item.Icon = $"avares://Nlnet.Avalonia.Css.App/Assets/Png/{item.ViewType.Name[..^4]}.png";
                }
                GalleryItems.AddRange(t.Result);
            });
        }

        protected override void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            switch (propertyName)
            {
                case nameof(Accent):
                {
                    var cfg = AcssContext.Default.GetService<IAcssConfiguration>();
                    cfg.Accent = Accent;

                    var cssTheme = Application.Current?.Styles.FirstOrDefault(s => s is AcssFluentTheme) as AcssFluentTheme;
                
                    cssTheme?.UpdateThemeColor(false);
                    break;
                }
                case nameof(Theme):
                {
                    if (Application.Current != null)
                    {
                        Application.Current.RequestedThemeVariant = Theme;
                    }

                    break;
                }
            }
        }


        
        #region Toggle Acss

        private bool _isBeforeLoadedAcssFileLoaded = true;

        public void ToggleAppAcssFile()
        {
            if (Application.Current is not App app)
            {
                return;
            }

            if (_isBeforeLoadedAcssFileLoaded)
            {
                app.AppCssFile?.Unload();
            }
            else
            {
                app.LoadAppCssFile();
            }
            _isBeforeLoadedAcssFileLoaded = !_isBeforeLoadedAcssFileLoaded;
        }

        #endregion



        #region Enable Transitions

        private bool _isTransitionsEnabled = true;

        public void ToggleTransitions()
        {
            _isTransitionsEnabled = !_isTransitionsEnabled;
            AcssContext.Default.EnableTransitions(_isTransitionsEnabled);
        }

        #endregion
    }
}
