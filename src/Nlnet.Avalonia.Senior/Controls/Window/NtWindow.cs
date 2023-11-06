using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Interactivity;
using Nlnet.Avalonia.Css;

// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.Senior.Controls
{
    [PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen")]
    public class NtWindow : Window
    {
        protected override Type StyleKeyOverride { get; } = typeof(NtWindow);

        private readonly CompositeDisposable? _disposables;

        public double TitleHeight
        {
            get { return GetValue(TitleHeightProperty); }
            set { SetValue(TitleHeightProperty, value); }
        }
        public static readonly StyledProperty<double> TitleHeightProperty = AvaloniaProperty
            .Register<NtWindow, double>(nameof(TitleHeight), 40d);

        public object? TitleContent
        {
            get { return GetValue(TitleContentProperty); }
            set { SetValue(TitleContentProperty, value); }
        }
        public static readonly StyledProperty<object?> TitleContentProperty = AvaloniaProperty
            .Register<NtWindow, object?>(nameof(TitleContent));

        public object? Mask
        {
            get { return GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }
        public static readonly StyledProperty<object?> MaskProperty = AvaloniaProperty
            .Register<NtWindow, object?>(nameof(Mask));

        public bool UseFullScreenButton
        {
            get { return GetValue(UseFullScreenButtonProperty); }
            set { SetValue(UseFullScreenButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseFullScreenButtonProperty =
            NtCaptionButtons.UseFullScreenButtonProperty.AddOwner<NtWindow>();

        public bool UseMaximizeButton
        {
            get { return GetValue(UseMaximizeButtonProperty); }
            set { SetValue(UseMaximizeButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseMaximizeButtonProperty = 
            NtCaptionButtons.UseMaximizeButtonProperty.AddOwner<NtWindow>();

        public bool UseMinimizeButton
        {
            get { return GetValue(UseMinimizeButtonProperty); }
            set { SetValue(UseMinimizeButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseMinimizeButtonProperty = 
            NtCaptionButtons.UseMinimizeButtonProperty.AddOwner<NtWindow>();

        public bool UseCloseButton
        {
            get { return GetValue(UseCloseButtonProperty); }
            set { SetValue(UseCloseButtonProperty, value); }
        }
        public static readonly StyledProperty<bool> UseCloseButtonProperty =
            NtCaptionButtons.UseCloseButtonProperty.AddOwner<NtWindow>();

        public bool UseCustomResizer
        {
            get { return GetValue(UseCustomResizerProperty); }
            set { SetValue(UseCustomResizerProperty, value); }
        }
        public static readonly StyledProperty<bool> UseCustomResizerProperty = AvaloniaProperty
            .Register<NtWindow, bool>(nameof(UseCustomResizer));
        


        public static bool GetIsHitTestVisibleInChrome(Visual host)
        {
            return host.GetValue(IsHitTestVisibleInChromeProperty);
        }
        public static void SetIsHitTestVisibleInChrome(Visual host, bool value)
        {
            host.SetValue(IsHitTestVisibleInChromeProperty, value);
        }
        public static readonly AttachedProperty<bool> IsHitTestVisibleInChromeProperty = AvaloniaProperty
            .RegisterAttached<NtWindow, Visual, bool>("IsHitTestVisibleInChrome", false, true);



        static NtWindow()
        {
            OwnerProperty.Changed.AddClassHandler<NtWindow>((win, args) =>
            {
                if (win.IsSet(OwnerProperty) == false)
                {
                    win.Icon = (args.NewValue as Window)?.Icon;
                }
            });
        }

        public NtWindow()
        {
            _disposables = new CompositeDisposable()
            {
                this.GetObservable(Window.WindowStateProperty)
                    .Subscribe(x =>
                    {
                        PseudoClasses.Set(":minimized",  x == WindowState.Minimized);
                        PseudoClasses.Set(":normal",     x == WindowState.Normal);
                        PseudoClasses.Set(":maximized",  x == WindowState.Maximized);
                        PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                    }),
            };
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            _disposables?.Dispose();
        }

        protected override void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);

            UpdatePosition();
        }

        protected void UpdatePosition()
        {
            if (WindowStartupLocation == WindowStartupLocation.Manual)
            {
                return;
            }

            var screen = Screens.ScreenFromWindow(this);
            if (screen == null)
            {
                return;
            }

            this.UpdateLayout();

            if (WindowState != WindowState.Normal)
            {
                return;
            }

            if (WindowStartupLocation == WindowStartupLocation.CenterScreen || this.Owner == null)
            {
                var x = screen.Bounds.TopLeft.X + (screen.Bounds.Width  - this.Bounds.Width  * screen.Scaling) / 2;
                var y = screen.Bounds.TopLeft.Y + (screen.Bounds.Height - this.Bounds.Height * screen.Scaling) / 2;

                if (this.Bounds.Width * screen.Scaling > screen.Bounds.Width)
                {
                    x = 0;
                }
                if (this.Bounds.Height * screen.Scaling > screen.Bounds.Height)
                {
                    y = 0;
                }

                DiagnosisHelper.WriteLine(
                    $"screen.Bounds.TopLeft.Y={screen.Bounds.TopLeft.Y}, "
                    + $"screen.Bounds.Height={screen.Bounds.Height}, "
                    + $"this.Bounds.Height={this.Bounds.Height}, "
                    + $"screen.Scaling={screen.Scaling}.");

                this.Position = new PixelPoint(Math.Abs((int)x), Math.Abs((int)y));
            }
            else
            {
                if (Owner is not Window window)
                {
                    return;
                }

                var x = window.Position.X + (Owner.Bounds.Width  - this.Bounds.Width)  / 2 * screen.Scaling;
                var y = window.Position.Y + (Owner.Bounds.Height - this.Bounds.Height) / 2 * screen.Scaling;

                if (this.Bounds.Width * screen.Scaling > screen.Bounds.Width)
                {
                    x = 0;
                }
                if (this.Bounds.Height * screen.Scaling > screen.Bounds.Height)
                {
                    y = 0;
                }

                DiagnosisHelper.WriteLine(
                    $"window.Position.Y={window.Position.Y}, "
                    + $"Owner.Bounds.Height={Owner.Bounds.Height}, "
                    + $"this.Bounds.Height={this.Bounds.Height}, "
                    + $"screen.Scaling={screen.Scaling}.");

                this.Position = new PixelPoint(Math.Abs((int)x), Math.Abs((int)y));
            }
        }
    }
}