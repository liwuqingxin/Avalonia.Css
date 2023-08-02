using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal class ChildStyle : Style
    {
        public IAcssStyle AcssStyle { get; }

        public ChildStyle(IAcssStyle acssStyle)
        {
            AcssStyle = acssStyle;
        }
    }

    internal class LogicChildStyle : ChildStyle
    {
        public LogicChildStyle(IAcssStyle acssStyle) : base(acssStyle)
        {

        }
    }
}
