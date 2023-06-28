using System;
using Avalonia.Animation;
using Avalonia.Animation.Animators;

namespace Nlnet.Avalonia.Css
{
    /// <summary>
    /// Animator that handles <see cref="T:Avalonia.Media.Transform" /> properties.
    /// </summary>
    public class RhythmicTransformAnimator : TransformAnimator
    {
        public RhythmicTransformAnimator()
        {
            
        }

        public override IDisposable BindAnimation(Animatable control, IObservable<double> instance)
        {
            int i = 0;
            return base.BindAnimation(control, instance);
        }
    }
}
