using System;
using System.Reactive.Disposables;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Chrome;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;

// ReSharper disable InconsistentNaming

namespace Nlnet.Avalonia.Senior.Controls
{
    [TemplatePart(PART_CaptionButtons, typeof(CaptionButtons))]
    [PseudoClasses(":minimized", ":normal", ":maximized", ":fullscreen")]
    public class NtTitleBar : ContentControl
    {
        private const string PART_CaptionButtons = nameof(PART_CaptionButtons);

        private CompositeDisposable? _disposables;
        private CaptionButtons? _captionButtons;

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
        {
            base.OnApplyTemplate(e);
            
            DoubleTapped += OnDoubleTapped;

            _captionButtons?.Detach();
            _captionButtons = e.NameScope.Get<CaptionButtons>(PART_CaptionButtons);
            if (VisualRoot is not Window visualRoot)
            {
                return;
            }
            _captionButtons?.Attach(visualRoot);
            UpdateSize(visualRoot);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            if (VisualRoot is not Window window)
            {
                return;
            }

            _disposables = new CompositeDisposable()
            {
                window.GetObservable(Window.WindowDecorationMarginProperty)
                    .Subscribe(x => UpdateSize(window)),
                window.GetObservable(Window.ExtendClientAreaTitleBarHeightHintProperty)
                    .Subscribe(x => UpdateSize(window)),
                window.GetObservable(Window.OffScreenMarginProperty)
                    .Subscribe(x => UpdateSize(window)),
                window.GetObservable(Window.ExtendClientAreaChromeHintsProperty)
                    .Subscribe(x => UpdateSize(window)),
                window.GetObservable(Window.IsExtendedIntoWindowDecorationsProperty)
                    .Subscribe(x => UpdateSize(window)),
                window.GetObservable(Window.WindowStateProperty)
                    .Subscribe(x =>
                    {
                        PseudoClasses.Set(":minimized",  x == WindowState.Minimized);
                        PseudoClasses.Set(":normal",     x == WindowState.Normal);
                        PseudoClasses.Set(":maximized",  x == WindowState.Maximized);
                        PseudoClasses.Set(":fullscreen", x == WindowState.FullScreen);
                    }),
            };
        }

        private void UpdateSize(Window? window)
        {
            if (window == null)
            {
                return;
            }
            
            // See NtTitleBar.UpdateSize(Window? window).
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            _disposables?.Dispose();
            _captionButtons?.Detach();
            _captionButtons = null;
        }

        protected override void OnPointerPressed(PointerPressedEventArgs e)
        {
            base.OnPointerPressed(e);

            if(e.Source is Visual visual && (Equals(visual, this) || Equals(visual.TemplatedParent, this)) && VisualRoot is Window window)
            {
                window.BeginMoveDrag(e);
            }
        }

        private void OnDoubleTapped(object? sender, TappedEventArgs e)
        {
            if (e.Source is Visual visual && NtWindow.GetIsHitTestVisibleInChrome(visual))
            {
                return;
            }

            if (this.VisualRoot is Window { CanResize: false })
            {
                return;
            }

            if (VisualRoot is Window window)
            {
                window.WindowState = window.WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;
            }

            e.Handled = true;
        }
    }
}
