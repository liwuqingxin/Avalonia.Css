using Avalonia;
using Avalonia.Data;
using Nlnet.Avalonia.Css.App.Views;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App
{
    internal class IndependentCase : Case
    {
        private ICssBuilder? _builder;
        private ICssFile?    _cssFile;

        public bool IsLocalDark
        {
            get { return GetValue(IsLocalDarkProperty); }
            set { SetValue(IsLocalDarkProperty, value); }
        }
        public static readonly StyledProperty<bool> IsLocalDarkProperty = AvaloniaProperty
            .Register<IndependentCase, bool>(nameof(IsLocalDark));

        static IndependentCase()
        {
            IsLocalDarkProperty.Changed.AddClassHandler<IndependentCase>((inCase, args) =>
            {
                inCase.UpdateModeResource();
            });
        }

        private void UpdateModeResource()
        {
            if (_builder == null)
            {
                _builder = new CssBuilder();
                _builder.Configuration.Mode = IsLocalDark ? "dark" : "light";
                _cssFile = _builder.BuildLoader().Load(this.Styles, "../../../Nlnet.Avalonia.Css.Fluent/Css/Resources/Mode.acss");
                return;
            }

            _builder.Configuration.Mode = IsLocalDark ? "dark" : "light";
            _cssFile?.Reload();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            this.Bind(IsLocalDarkProperty, new Binding($"DataContext.{nameof(MainWindowViewModel.IsLocalDark)}")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                {
                    AncestorType = typeof(MainWindow),
                }
            });
        }
    }
}
