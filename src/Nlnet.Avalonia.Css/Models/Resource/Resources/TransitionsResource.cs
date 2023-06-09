using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
public class TransitionResource : CssResource<TransitionResource>
{
    protected override object? Accept(string valueString)
    {
        return InterpreterHelper.ParseTransition(valueString);
    }
}