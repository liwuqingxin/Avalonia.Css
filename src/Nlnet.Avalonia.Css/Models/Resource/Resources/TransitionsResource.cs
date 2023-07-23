using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
internal class TransitionResource : CssResourceBaseAndFac<TransitionResource>
{
    protected override object? Accept(ICssBuilder cssBuilder, string valueString)
    {
        return cssBuilder.Interpreter.ParseTransition(valueString);
    }
}