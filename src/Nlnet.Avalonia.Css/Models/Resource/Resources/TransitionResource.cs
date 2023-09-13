using System;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType("Transition")]
internal class TransitionResource : AcssResourceBaseAndFac<TransitionResource>
{
    protected override object? BuildValue(IAcssBuilder acssBuilder, string valueString)
    {
        var app        = Checks.CheckApplication();
        var transition = acssBuilder.Interpreter.ParseTransition(valueString, app);
        return transition;
    }
}