﻿using System;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css
{
    internal interface IAcssAnimation : IAcssSection
    {
        IAnimation? ToAvaloniaAnimation();
    }

    internal class AcssAnimation : AcssSection, IAcssAnimation
    {
        private static readonly Regex RegexDescription = new("\\[desc=(.*?)\\]", RegexOptions.IgnoreCase);

        private readonly IAcssBuilder _builder;

        private Animation? _animation;

        public string? Description { get; set; }

        public AcssAnimation(IAcssBuilder builder, string selector) : base(builder, selector)
        {
            _builder = builder;
        }

        public override void InitialSection(IAcssParser parser, ReadOnlySpan<char> content)
        {
            var matchDesc = RegexDescription.Match(Selector);
            if (matchDesc.Success)
            {
                Description = matchDesc.Groups[1].Value;
            }

            if (Parent is not IAcssStyle style)
            {
                this.WriteError($"The parent of {nameof(AcssAnimation)} must be {nameof(AcssStyle)}. Skip it.");
                return;
            }
            
            var selectorTargetType = style.GetTargetType();
            if (selectorTargetType == null)
            {
                this.WriteError($"The target type of the style is null. Skip it [{Description}].");
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

        public override string ToString()
        {
            return $"{typeof(AcssAnimation)} [{Description}]";
        }
    }
}
