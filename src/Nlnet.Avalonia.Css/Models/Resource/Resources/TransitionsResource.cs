using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
public class TransitionResource : CssResourceBaseAndFac<TransitionResource>
{
    protected override object? Accept(string valueString)
    {
        return CssServiceLocator.GetService<ICssInterpreter>().ParseTransition(valueString);
    }
}