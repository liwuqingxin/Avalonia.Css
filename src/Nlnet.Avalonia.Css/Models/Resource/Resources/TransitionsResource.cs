using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
internal class TransitionResource : AcssResourceBaseAndFac<TransitionResource>
{
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        return acssBuilder.Interpreter.ParseTransition(valueString);
    }
}