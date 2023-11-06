using Avalonia.Media;

namespace Nlnet.Avalonia.Css;

[ResourceType("Linear")]
[ResourceType(nameof(LinearGradientBrush))]
internal class LinearResource : AcssResourceBaseAndFac<LinearResource>
{
    protected override object? BuildValue(IAcssContext context, string valueString)
    {
        var interpreter = context.GetService<IAcssInterpreter>();

        valueString = valueString.Trim();
        var app = Checks.CheckApplication();
        if (valueString.StartsWith("("))
        {
            var brush = interpreter.ParseLinear(valueString, app);
            return brush?.ToImmutable();
        }
        else if (valueString.StartsWith("{"))
        {
            var brush = interpreter.ParseComplexLinear(valueString, app);
            return brush?.ToImmutable();
        }
        else
        {
            return null;
        }
    }
}