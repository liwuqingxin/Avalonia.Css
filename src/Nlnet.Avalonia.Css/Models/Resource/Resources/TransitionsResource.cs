using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
public class TransitionsResource : CssResource<TransitionsResource>
{
    protected override object? Accept(string valueString)
    {
        return InterpreterHelper.ParseTransition(valueString);
    }
}
