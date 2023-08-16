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
            .Register<NtScrollViewer, double>(nameof(SmoothScrollingStep), 200d);

        public bool UseCustomScrollAnimation
        {
            get { return GetValue(UseCustomScrollAnimationProperty); }
            set { SetValue(UseCustomScrollAnimationProperty, value); }
        }
        public static readonly StyledProperty<bool> UseCustomScrollAnimationProperty = AvaloniaProperty
            .Register<NtScrollViewer, bool>(nameof(UseCustomScrollAnimation));
    }
}
