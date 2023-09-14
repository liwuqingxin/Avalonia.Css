using System;
using System.Collections.Generic;
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
        private List<(string, string)>? _setters;
        private Type? _selectorTargetType;

        public string? Description { get; set; }

        public AcssAnimation(IAcssBuilder builder, string selector) : base(builder, selector)
        {
            _builder = builder;
        }

        public override void InitialSection(IAcssParser parser, ReadOnlySpan<char> content)
        {
            var matchDesc = RegexDescription.Match(Header);
            if (matchDesc.Success)
            {
                Description = matchDesc.Groups[1].Value;
            }

            if (Parent is not IAcssStyle style)
            {
                this.WriteError($"The parent of {nameof(AcssAnimation)} must be {nameof(AcssStyle)}. Skip it.");
                return;
            }

            _selectorTargetType = style.GetTargetType();
            if (_selectorTargetType == null)
            {
                this.WriteError($"The target type of the style is null. Skip it [{Description}].");
                return;
            }

            _setters = parser.ParsePairs(content).ToList();
        }

        public IAnimation? ToAvaloniaAnimation()
        {
            var animation = new Animation();
            var interpreter = _builder.Interpreter;

            animation.ApplySetters(interpreter, _setters!, nameof(Animation.Children));

            var childrenSetter = _setters!.FirstOrDefault(s => s.Item1 is nameof(Animation.Children) or nameof(KeyFrames));
            var keyFrames = interpreter.ParseKeyFrames(_selectorTargetType!, childrenSetter.Item2)?.ToList();
            if (keyFrames == null || keyFrames.Count == 0)
            {
                this.WriteWarning($"No key frames detected in animation '{Description}'.");
                return null;
            }
            else
            {
                animation.Children.AddRange(keyFrames);
            }

            return animation;
        }

        public override string ToString()
        {
            return $"{typeof(AcssAnimation)} [{Description}]";
        }
    }
}
