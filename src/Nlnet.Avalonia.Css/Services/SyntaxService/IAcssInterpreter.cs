using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css;

internal interface IAcssInterpreter : IService
{
    public Selector? ToSelector(IAcssContext context, IAcssStyle acssStyle, IEnumerable<ISyntax> syntaxList);

    public AvaloniaProperty? ParseAvaloniaProperty(Type avaloniaObjectType, string property);

    public AvaloniaProperty? ParseAcssBehaviorProperty(Type avaloniaObjectType, string property, string? rawValue, out AcssBehavior? value);

    public object? ParseClrValue(Type declaredType, string? rawValue);
    
    public object? ParseValue(AvaloniaProperty avaloniaProperty, string? rawValue);

    public object? ParseDynamicValue(AvaloniaProperty avaloniaProperty, string? rawValue);
    
    public object? ParseStaticValue(AvaloniaProperty avaloniaProperty, string? rawValue);

    public bool IsVar(string? valueString, out string? varKey);

    public bool IsBinding(string? valueString, out Binding? binding);

    public ITransition? ParseTransition(string valueString, IResourceHost host);

    public IEnumerable<KeyFrame>? ParseKeyFrames(Type selectorTargetType, string valueString);

    public LinearGradientBrush? ParseLinear(string valueString, IResourceHost host);

    public LinearGradientBrush? ParseComplexLinear(string valueString, IResourceHost host);

    public string ParseSelectorAndBases(string header, out IList<string>? bases);
}
