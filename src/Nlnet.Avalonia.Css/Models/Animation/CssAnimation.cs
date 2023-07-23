using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Media;
using DynamicData;

namespace Nlnet.Avalonia.Css
{
    internal interface ICssAnimation : ICssSection
    {
        IAnimation? ToAvaloniaAnimation();
    }

    internal class CssAnimation : CssSection, ICssAnimation
    {
        private static readonly Regex RegexDescription = new("\\[desc=(.*?)\\]", RegexOptions.IgnoreCase);

        private readonly ICssBuilder _builder;

        private Animation? _animation;

        public string? Description { get; set; }

        public CssAnimation(ICssBuilder builder, string selector) : base(builder, selector)
        {
            _builder = builder;
        }

        public override void InitialSection(ICssParser parser, ReadOnlySpan<char> content)
        {
            var matchDesc = RegexDescription.Match(Selector);
            if (matchDesc.Success)
            {
                Description = matchDesc.Groups[1].Value;
            }

            if (Parent is not ICssStyle style)
            {
                this.WriteError($"The parent of {nameof(CssAnimation)} must be {nameof(CssStyle)}. Skip it.");
                return;
            }
            if (style.GetSelector() is not { } selector)
            {
                this.WriteError($"The style is invalid as Selector of it is null. Skip it.");
                return;
            }

            var selectorTargetType = selector.GetTargetType() ?? style.GetParentTargetType();
            if (selectorTargetType == null)
            {
                this.WriteError($"The target type of the style is null. Skip it.");
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
                    this.WriteError($"Can not get the property '{setter.Item1}' from type of {nameof(Animation)}. Skip it.");
                    continue;
                }

                var value = interpreter.ParseValue(property.PropertyType, setter.Item2);
                property.SetValue(_animation, value);
            }

            var childrenSetter = setters.FirstOrDefault(s => s.Item1 is nameof(Animation.Children) or nameof(KeyFrames));
            var keyFrames      = interpreter.ParseKeyFrames(selectorTargetType, childrenSetter.Item2)?.ToList();
            if (keyFrames == null)
            {
                this.WriteWarning($"No key frames detected in animation '{Description}'.");
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
