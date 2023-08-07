
using System;
using System.Reflection;
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
        
        if (_keyDuration != null && acssBuilder.ResourceProvidersManager.TryFindResource(_keyDuration, out var resource1))
        {
            var durationProp = _transition.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public);
            switch (resource1)
            {
                case double d:
                    durationProp?.SetValue(_transition, TimeSpan.FromSeconds(d));
                    break;
                case TimeSpan t:
                    durationProp?.SetValue(_transition, t);
                    break;
            }
        }
        
        if(_keyDelay != null && acssBuilder.ResourceProvidersManager.TryFindResource(_keyDelay, out var resource2))
        {
            var delayProp    = _transition.GetType().GetProperty("Delay",    BindingFlags.Instance | BindingFlags.Public);
            switch (resource2)
            {
                case double d:
                    delayProp?.SetValue(_transition, TimeSpan.FromSeconds(d));
                    break;
                case TimeSpan t:
                    delayProp?.SetValue(_transition, t);
                    break;
            }
        }

        if (_keyEasing != null && acssBuilder.ResourceProvidersManager.TryFindResource(_keyEasing, out var resource3))
        {
            var easingProp = _transition.GetType().GetProperty("Easing",   BindingFlags.Instance | BindingFlags.Public);
            easingProp?.SetValue(_transition, resource3);
        }
        
        return _transition;
    }
}