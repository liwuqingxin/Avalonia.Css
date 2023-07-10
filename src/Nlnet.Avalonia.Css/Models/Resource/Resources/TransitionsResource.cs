using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
internal class TransitionResource : CssResourceBaseAndFac<TransitionResource>
{
    protected override object? Accept(string valueString)
    {
        return ServiceLocator.GetService<ICssInterpreter>().ParseTransition(valueString);
    }
}