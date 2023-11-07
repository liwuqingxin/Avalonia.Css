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
        
        private readonly IAcssContext _context;
        private List<(string, string)>? _setters;
        private Type? _selectorTargetType;

        public string? Description { get; set; }

        public AcssAnimation(IAcssContext context, string header) : base(context, header)
        {
            _context = context;
        }

        public override void InitialSection(IAcssParser parser, ReadOnlySpan<char> content)
        {
            var matchDesc = RegexDescription.Match(Header);
            if (matchDesc.Success)
            {
                Description = matchDesc.Groups[1].Value;
            }

            _setters = parser.ParsePairs(content).ToList();
        }

        public override IAcssSection Clone()
        {
            var acssAnimation = new AcssAnimation(Context, Header)
            {
                _setters = _setters,
                _selectorTargetType = _selectorTargetType,
                Description = Description,
                Parent = Parent,
                Children = Children,
            };
            
            return acssAnimation;
        }

        public IAnimation? ToAvaloniaAnimation()
        {
            if (Parent is not IAcssStyle style)
            {
                _context.OnError(AcssErrors.Animation_With_Invalid_Parent,
                    $"The parent of the animation '{Description}' must be {nameof(AcssStyle)}. Skip it.");

                return null;
            }

            _selectorTargetType = style.GetTargetType();
            if (_selectorTargetType == null)
            {
                _context.OnError(AcssErrors.Animation_With_Invalid_Target_Type,
                    $"The target type of the animation '{Description}' is null. Skip it.");

                return null;
            }
            
            var animation = new Animation();
            var interpreter = Context.GetService<IAcssInterpreter>();

            animation.ApplySetters(interpreter, _setters!, nameof(Animation.Children));

            var childrenSetter = _setters!.FirstOrDefault(s => s.Item1 is nameof(Animation.Children) or nameof(KeyFrames));
            var keyFrames = interpreter.ParseKeyFrames(_selectorTargetType!, childrenSetter.Item2)?.ToList();
            if (keyFrames == null || keyFrames.Count == 0)
            {
                _context.OnError(AcssErrors.Animation_With_Empty_Frames,
                    $"No key frames detected in the animation '{Description}'. Skip it.");

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
