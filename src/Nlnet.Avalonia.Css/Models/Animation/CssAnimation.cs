using System;
using System.Linq;
using Avalonia.Animation;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css
{
    internal interface ICssAnimation : ICssSection
    {
        IAnimation? ToAvaloniaAnimation();
    }

    internal class CssAnimation : CssSection, ICssAnimation
    {
        private readonly ICssBuilder _builder;

        private Animation? _animation;

        public CssAnimation(ICssBuilder builder, string selector) : base(builder, selector)
        {
            _builder = builder;
        }

        public override void InitialSection(ICssParser parser, ReadOnlySpan<char> content)
        {
            if (Parent is not ICssStyle style)
            {
                this.WriteLine($"The parent of {nameof(CssAnimation)} must be {nameof(CssStyle)}. Skip it.");
                return;
            }
            if (style.GetSelector() is not { } selector)
            {
                this.WriteLine($"The style is invalid as Selector of it is null. Skip it.");
                return;
            }
            if (selector.TargetType == null)
            {
                this.WriteLine($"The target type of the selector of the style is null. Skip it.");
                return;
            }

            _animation = new Animation();

            var interpreter = _builder.Interpreter;
            var type        = typeof(Animation);
            var setters     = parser.ParsePairs(content).ToList();
            foreach (var setter in setters.Where(s => s.Item1 != nameof(Animation.Children)))
            {
                var property = type.GetProperty(setter.Item1);
                if (property == null)
                {
                    continue;
                }

                var value = interpreter.ParseValue(property.PropertyType, setter.Item2);
                property.SetValue(_animation, value);
            }

            var childrenSetter = setters.FirstOrDefault(s => s.Item1 is nameof(Animation.Children) or nameof(KeyFrames));
            var keyFrames      = interpreter.ParseKeyFrames(selector.TargetType, childrenSetter.Item2)?.ToList();
            if (keyFrames == null)
            {
                _animation = null;
            }
            else
            {
                _animation.Children.AddRange(keyFrames);
            }
        }

        public IAnimation? ToAvaloniaAnimation()
        {
            return _animation;
        }
    }
}
