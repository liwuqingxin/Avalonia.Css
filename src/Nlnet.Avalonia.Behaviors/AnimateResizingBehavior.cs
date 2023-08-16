using System;
using System.Threading;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;

namespace Nlnet.Avalonia.Behaviors
{
    public class AnimateResizingBehavior : Behavior<Control>
    {
        public static double GetAnimatingWidth(Control host)
        {
            return host.GetValue(AnimatingWidthProperty);
        }
        public static void SetAnimatingWidth(Control host, double value)
        {
            host.SetValue(AnimatingWidthProperty, value);
        }
        public static readonly AttachedProperty<double> AnimatingWidthProperty = AvaloniaProperty
            .RegisterAttached<AnimateResizingBehavior, Control, double>("AnimatingWidth");

        public static double GetActualWidth(Control host)
        {
            return host.GetValue(ActualWidthProperty);
        }
        public static void SetActualWidth(Control host, double value)
        {
            host.SetValue(ActualWidthProperty, value);
        }
        public static readonly AttachedProperty<double> ActualWidthProperty = AvaloniaProperty
            .RegisterAttached<AnimateResizingBehavior, Control, double>("ActualWidth");





        protected override void OnAttached()
        {
            base.OnAttached();

            if (AssociatedObject != null)
            {
                AssociatedObject.SizeChanged -= AssociatedObjectOnSizeChanged;
                AssociatedObject.SizeChanged += AssociatedObjectOnSizeChanged;
                AssociatedObject.Transitions = new Transitions()
                {
                    new DoubleTransition()
                    {
                        Property = AnimateResizingBehavior.AnimatingWidthProperty,
                        Duration = TimeSpan.FromMilliseconds(200),
                    }
                };
                SetAnimatingWidth(AssociatedObject, AssociatedObject.Bounds.Width);
            }

            AnimatingWidthProperty.Changed.Subscribe(OnNext);
        }

        private void AssociatedObjectOnSizeChanged(object? sender, SizeChangedEventArgs e)
        {
            if (AssociatedObject == null || AssociatedObject.IsAnimating(AnimatingWidthProperty))
            {
                return;
            }

            SetActualWidth(AssociatedObject, e.PreviousSize.Width);
            SetActualWidth(AssociatedObject, AssociatedObject.Bounds.Width);
            SetAnimatingWidth(AssociatedObject, AssociatedObject.Bounds.Width);
        }

        private void OnNext(AvaloniaPropertyChangedEventArgs<double> obj)
        {
            if (AssociatedObject != null)
            {
                AssociatedObject.Width = obj.NewValue.Value;
                if (Math.Abs(GetActualWidth(AssociatedObject) - obj.NewValue.Value) < 0.001)
                {
                    AssociatedObject.Width = double.NaN;
                }
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if (AssociatedObject != null)
            {
                AssociatedObject.SizeChanged -= AssociatedObjectOnSizeChanged;
            }
        }
    }
}
