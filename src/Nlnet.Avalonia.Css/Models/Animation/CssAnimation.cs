using System;

namespace Nlnet.Avalonia.Css
{
    public interface ICssAnimation : ICssSection
    {

    }

    public class CssAnimation : CssSection, ICssAnimation
    {
        public CssAnimation(string selector) : base(selector)
        {
        }

        public override void InitialSection(ICssParser parser, ReadOnlySpan<char> content)
        {
        }
    }
}
