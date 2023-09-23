using System;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType("Transition")]
internal class TransitionResource : AcssResourceBaseAndFac<TransitionResource>
{
    protected override object? BuildValue(IAcssContext context, string valueString)
    {
        var interpreter = context.GetService<IAcssInterpreter>();
        var app        = Checks.CheckApplication();
        var transition = interpreter.ParseTransition(valueString, app);
        return transition;
    }
}