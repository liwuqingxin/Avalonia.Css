using System;
using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.SampleAssistant
{
    public class Case : HeaderedContentControl
    {
        protected override Type StyleKeyOverride { get; } = typeof(Case);



        public string Xaml
        {
            get { return GetValue(XamlProperty); }
            set { SetValue(XamlProperty, value); }
        }
        public static readonly StyledProperty<string> XamlProperty = AvaloniaProperty
            .Register<Case, string>(nameof(Xaml));

        public string? Description
        {
            get { return GetValue(DescriptionProperty); }
            set { SetValue(DescriptionProperty, value); }
        }
        public static readonly StyledProperty<string?> DescriptionProperty = AvaloniaProperty
            .Register<Case, string?>(nameof(Description));

        public object Settings
        {
            get { return GetValue(SettingsProperty); }
            set { SetValue(SettingsProperty, value); }
        }
        public static readonly StyledProperty<object> SettingsProperty = AvaloniaProperty
            .Register<Case, object>(nameof(Settings));

        public bool UseSourceCode
        {
            get { return GetValue(UseSourceCodeProperty); }
            set { SetValue(UseSourceCodeProperty, value); }
        }
        public static readonly StyledProperty<bool> UseSourceCodeProperty = AvaloniaProperty
            .Register<Case, bool>(nameof(UseSourceCode), true);



        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            var header = Header?.ToString();
            if (header == null)
            {
                return;
            }

            var xamlProvider = this.FindAncestorOfType<IXamlProvider>();
            if (xamlProvider != null)
            {
                Xaml = xamlProvider.ProvideXaml(header) ?? string.Empty;
            }
        }
    }
}
