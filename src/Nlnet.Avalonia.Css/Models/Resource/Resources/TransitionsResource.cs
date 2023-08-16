using System;
using Avalonia.Animation;

namespace Nlnet.Avalonia.Css;

[ResourceType(nameof(Transition<object>))]
internal class TransitionResource : AcssResourceBaseAndFac<TransitionResource>
{
    private ITransition? _transition;
    private string? _keyDuration;
    private string? _keyDelay;
    private string? _keyEasing;
    
    protected override object? Accept(IAcssBuilder acssBuilder, string valueString)
    {
        _transition = acssBuilder.Interpreter.ParseTransition(valueString, out var shouldDefer, out _keyDuration, out _keyDelay, out _keyEasing);
        IsDeferred = shouldDefer;
        return _transition;
    }

    public override object? GetDeferredValue(IAcssBuilder acssBuilder, IServiceProvider? provider)
    {
        if (_transition == null)
        {
            return _transition;
        }

        _transition.ApplyVarForDuration(acssBuilder, _keyDuration);
        _transition.ApplyVarForDelay(acssBuilder, _keyDelay);
        _transition.ApplyVarForEasing(acssBuilder, _keyEasing);

        return _transition;
    }
}