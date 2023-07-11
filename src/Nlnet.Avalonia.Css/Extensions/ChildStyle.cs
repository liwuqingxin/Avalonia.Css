using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal class ChildStyle : Style
    {
        public ICssStyle CssStyle { get; }

        public ChildStyle(ICssStyle cssStyle)
        {
            CssStyle = cssStyle;
        }

        public override SelectorMatchResult TryAttach(IStyleable target, object? host)
        {
            var result = base.TryAttach(target, host);

            return result;
        }
    }

    internal class LogicChildStyle : ChildStyle
    {
        public LogicChildStyle(ICssStyle cssStyle) : base(cssStyle)
        {

        }
    }
}
