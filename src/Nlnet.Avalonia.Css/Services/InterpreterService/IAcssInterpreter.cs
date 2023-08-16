using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface IAcssInterpreter
{
    public Selector? ToSelector(IAcssBuilder builder, IAcssStyle acssStyle, IEnumerable<ISyntax> syntaxList);

    public AvaloniaProperty? ParseAvaloniaProperty(Type avaloniaObjectType, string property);

    public AvaloniaProperty? ParseAcssBehaviorProperty(Type avaloniaObjectType, string property, string? rawValue, out AcssBehavior? value);

    public object? ParseValue(Type declaredType, string? rawValue);

    public object? ParseValue(AvaloniaProperty avaloniaProperty, string? rawValue);

    public bool IsVar(string? valueString, out string? varKey);

    public bool IsBinding(string? valueString, out Binding? binding);

    public ITransition? ParseTransition(string valueString, out bool shouldDefer, out string? keyDuration, out string? keyDelay, out string? keyEasing);

    public IEnumerable<KeyFrame>? ParseKeyFrames(Type selectorTargetType, string valueString);

    public LinearGradientBrush? ParseLinear(string valueString, out bool shouldDefer, out IEnumerable<(string,double)>? keys);

    public LinearGradientBrush? ParseComplexLinear(string valueString, out bool shouldDefer, out IEnumerable<(string,double)>? keys);
}
