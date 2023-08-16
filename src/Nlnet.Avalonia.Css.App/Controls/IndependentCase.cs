using System;
using Avalonia;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Styling;
using Nlnet.Avalonia.Css.App.Views;
using Nlnet.Avalonia.SampleAssistant;

namespace Nlnet.Avalonia.Css.App
{
    internal class IndependentCase : Case
    {
        protected override Type StyleKeyOverride { get; } = typeof(Case);

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
            this.ThemeVariant = IsLocalDark ? ThemeVariant.Dark : ThemeVariant.Light;
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

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
