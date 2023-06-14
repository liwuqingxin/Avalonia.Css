using System;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css
{
    public interface ICssAnimation : ICssSection
    {
        IAnimation? ToAvaloniaAnimation();
    }

    public class CssAnimation : CssSection, ICssAnimation
    {
        private Animation? _animation;

        public CssAnimation(string selector) : base(selector)
        {
        }

        public override void InitialSection(ICssParser parser, ReadOnlySpan<char> content)
        {
            _animation = new Animation();
            var type    = typeof(Animation);
            var setters = parser.ParsePairs(content);
            foreach (var setter in setters)
            {
                var property = type.GetProperty(setter.Item1);
                if (property == null)
                {
                    continue;
                }

                var value = ServiceLocator.GetService<ICssInterpreter>().ParseValue(property.PropertyType, setter.Item2);
                property.SetValue(_animation, value);
            }
        }

        public IAnimation? ToAvaloniaAnimation()
        {
            return _animation;
        }
    }
}
