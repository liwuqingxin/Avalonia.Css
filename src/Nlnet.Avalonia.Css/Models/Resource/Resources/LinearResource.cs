using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType("Linear")]
[ResourceType(nameof(LinearGradientBrush))]
internal class LinearResource : AcssResourceBaseAndFac<LinearResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        valueString = valueString.Trim();
        var app = Checks.CheckApplication();
        if (valueString.StartsWith("("))
        {
            var brush = acssBuilder.Interpreter.ParseLinear(valueString, app);
            return brush?.ToImmutable();
        }
        else if (valueString.StartsWith("{"))
        {
            var brush = acssBuilder.Interpreter.ParseComplexLinear(valueString, app);
            return brush?.ToImmutable();
        }
        else
        {
            return null;
        }
    }
}