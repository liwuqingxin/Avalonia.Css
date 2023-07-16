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
    }

    internal class LogicChildStyle : ChildStyle
    {
        public LogicChildStyle(ICssStyle cssStyle) : base(cssStyle)
        {

        }
    }
}
