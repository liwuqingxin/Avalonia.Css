using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Animators;
using Avalonia.Controls;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Rhythmic Animator that handles <see cref="T:Avalonia.Media.Transform" /> properties.
    /// </summary>
    internal sealed class RhythmicTransformAnimator : TransformAnimator
    {
        public double Step { get; set; } = 30;

        public double Pow { get; set; } = 0.96;

        public Type? ItemElementType { get; set; } = typeof(TabItem);

        public RhythmicTransformAnimator()
        {
            
        }

        public override IDisposable? Apply(Animation animation, Animatable control, IClock? clock, IObservable<bool> obsMatch, Action? onComplete)
        {
            if (control is not Visual visual || ItemElementType == null)
            {
                return base.Apply(animation, control, clock, obsMatch, onComplete);
            }

            var itemElement = FindAncestorOfType(visual, ItemElementType) ?? FindLogicalAncestorOfType(visual, ItemElementType);
            var panel       = itemElement?.FindAncestorOfType<Panel>()    ?? itemElement?.FindLogicalAncestorOfType<Panel>();

            if (itemElement == null || panel == null)
            {
                return base.Apply(animation, control, clock, obsMatch, onComplete);
            }

            var index          = panel.Children.IndexOf(itemElement);
            var step           = Step * Math.Pow(Pow, index);
            var animationClone = CloneAnimation(animation);

            animationClone.Delay = TimeSpan.FromMilliseconds(animation.Delay.TotalMilliseconds + step * index);
            animationClone.Duration = TimeSpan.FromMilliseconds(animation.Delay.TotalMilliseconds + animation.Duration.TotalMilliseconds + step * index);
            
            var disposable = base.Apply(animationClone, control, clock, obsMatch, onComplete);

            return disposable;
        }

        private static Animation CloneAnimation(Animation animation)
        {
            var clone  = new Animation();
            foreach (var animationChild in animation.Children)
            {
                clone.Children.Add(animationChild);
            }
            clone.Delay                  = animation.Delay;
            clone.DelayBetweenIterations = animation.DelayBetweenIterations;
            clone.Duration               = animation.Duration;
            clone.Easing                 = animation.Easing;
            clone.FillMode               = animation.FillMode;
            clone.IterationCount         = animation.IterationCount;
            clone.PlaybackDirection      = animation.PlaybackDirection;
            clone.SpeedRatio             = animation.SpeedRatio;

            return clone;
        }
        
        private static IControl? FindAncestorOfType(IVisual? visual, Type type, bool includeSelf = false)
        {
            if (visual is null)
            {
                return null;
            }

            var parent = includeSelf ? visual : visual.VisualParent;

            while (parent != null)
            {
                if (parent.GetType().IsAssignableTo(type) && parent is IControl element)
                {
                    return element;
                }

                parent = parent.VisualParent;
            }

            return null;
        }
        
        private static IControl? FindLogicalAncestorOfType(ILogical? logical, Type type, bool includeSelf = false)
        {
            if (logical == null)
            {
                return null;
            }

            for (var logical1 = includeSelf ? logical : logical.LogicalParent; logical1 != null; logical1 = logical1.LogicalParent)
            {
                if (logical1.GetType().IsAssignableTo(type) && logical1 is IControl element)
                {
                    return element;
                }
            }

            return null;
        }


        public static RhythmicTransformAnimator Parse(string text)
        {
            var instance = new RhythmicTransformAnimator();

            // eg. ""

            return instance;
        }
    }
}
