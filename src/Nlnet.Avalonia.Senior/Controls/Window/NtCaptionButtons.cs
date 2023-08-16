using Avalonia;
using Avalonia.Controls.Chrome;

namespace Nlnet.Avalonia.Senior.Controls
{
    public class NtCaptionButtons : CaptionButtons
    {
        public bool UseFullScreenButton
        {
            get { return GetValue(UseFullScreenButtonProperty); }
            set { SetValue(UseFullScreenButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseFullScreenButtonProperty = AvaloniaProperty
            .Register<NtCaptionButtons, bool>(nameof(UseFullScreenButton));

        public bool UseMaximizeButton
        {
            get { return GetValue(UseMaximizeButtonProperty); }
            set { SetValue(UseMaximizeButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseMaximizeButtonProperty = AvaloniaProperty
            .Register<NtCaptionButtons, bool>(nameof(UseMaximizeButton), true);

        public bool UseMinimizeButton
        {
            get { return GetValue(UseMinimizeButtonProperty); }
            set { SetValue(UseMinimizeButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseMinimizeButtonProperty = AvaloniaProperty
            .Register<NtCaptionButtons, bool>(nameof(UseMinimizeButton), true);

        public bool UseCloseButton
        {
            get { return GetValue(UseCloseButtonProperty); }
            set { SetValue(UseCloseButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseCloseButtonProperty = AvaloniaProperty
            .Register<NtCaptionButtons, bool>(nameof(UseCloseButton), true);
    }
}