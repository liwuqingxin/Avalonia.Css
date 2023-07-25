using System;
using Avalonia;
using Avalonia.Controls;

namespace Nlnet.Avalonia.Senior.Controls
{
    public class NtScrollViewer : ScrollViewer
    {
        protected override Type StyleKeyOverride { get; } = typeof(NtScrollViewer);

        public bool UseSmoothScrolling
        {
            get { return GetValue(UseSmoothScrollingProperty); }
            set { SetValue(UseSmoothScrollingProperty, value); }
        }
        public static readonly StyledProperty<bool> UseSmoothScrollingProperty = AvaloniaProperty
            .Register<NtScrollViewer, bool>(nameof(UseSmoothScrolling), true);

        public double SmoothScrollingStep
        {
            get { return GetValue(SmoothScrollingStepProperty); }
            set { SetValue(SmoothScrollingStepProperty, value); }
        }
        public static readonly StyledProperty<double> SmoothScrollingStepProperty = AvaloniaProperty
            .Register<NtScrollViewer, double>(nameof(SmoothScrollingStep), 400d);

        public bool UseAutoHide
        {
            get { return GetValue(UseAutoHideProperty); }
            set { SetValue(UseAutoHideProperty, value); }
        }
        public static readonly StyledProperty<bool> UseAutoHideProperty = AvaloniaProperty
            .Register<NtScrollViewer, bool>(nameof(UseAutoHide), true);
    }
}
