using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Media;
using Avalonia.Reactive;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal class AcssInterpreter : IAcssInterpreter
    {
        private readonly IAcssBuilder _builder;

        // ' var (xxx) '
        private readonly Regex _varRegex = new("^\\s*var\\s*\\((.*?)\\)\\s*$", RegexOptions.IgnoreCase);
        // ' $xxx.xxx ' or '$xxx#10.xxx '
        private readonly Regex _bindingRegex = new("^\\s*\\$([a-zA-Z0-9_]+)#?([0-9]*)\\.(.*?)\\s*$", RegexOptions.IgnoreCase);
        // ' @xxx.xxx '
        private readonly Regex _staticInstanceRegex = new("^\\s*@([a-zA-Z0-9_]+)\\.([a-zA-Z0-9_]+)\\s*$", RegexOptions.IgnoreCase);
        // ' xxx (xxx) '
        private readonly Regex _transitionRegex = new("([a-zA-Z]+)\\((.*)\\)", RegexOptions.IgnoreCase);
        // ' KeyFrame (xxx) : '
        private readonly Regex _keyFrameRegex = new("^\\s*KeyFrame\\s*\\:\\((.*?)\\)\\s*$", RegexOptions.IgnoreCase);
        // ' xxx (xxx) '
        private readonly Regex _setterAnimatorRegex = new("\\s*(.*?)\\s*\\(([a-zA-Z0-9_]*)\\)\\s*");
        // ' (x x x x) [ #cccccc 0.3 0.2; var(AccentColor) 1.2; ] '
        private readonly Regex _linearRegex = new("\\(\\s*(.*?)\\s+(.*?)\\s+(.*?)\\s+(.*?)\\s*\\)\\s*\\[\\s*(.*)\\s*\\]");
        // ' selector @extend ( content )', where content : 'base1, base2...'
        private readonly Regex _basesRegex = new("(.*?)\\s*@extend\\s*\\(\\s*(.*?)\\s*\\)");

        
        private readonly Dictionary<string, Type> _transitionsTypes;

        public AcssInterpreter(IAcssBuilder builder)
        {
            _builder          = builder;
            _transitionsTypes = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            var types = typeof(Transition<>)
                        .Assembly
                        .GetTypes()
                        .Where(t => t.IsAssignableTo(typeof(ITransition)) && t.IsAbstract == false);

            foreach (var t in types)
            {
                _transitionsTypes.Add(t.Name, t);
                _transitionsTypes.Add(t.Name[..^10], t);
            }

            _transitionsTypes.Add("corner", typeof(CornerRadiusTransition));
            _transitionsTypes.Add("int", typeof(IntegerTransition));
            _transitionsTypes.Add("thick", typeof(ThicknessTransition));
            _transitionsTypes.Add("shadow", typeof(ThicknessTransition));
        }

        public Selector? ToSelector(IAcssBuilder builder, IAcssStyle acssStyle, IEnumerable<ISyntax> syntaxList)
        {
            return syntaxList.Aggregate<ISyntax, Selector?>(null, (current, syntax) => syntax.ToSelector(builder, acssStyle, current));
        }

        public AvaloniaProperty? ParseAvaloniaProperty(Type avaloniaObjectType, string property)
        {
            if (property.StartsWith(BehaviorConstraints.AddToken) || property.StartsWith(BehaviorConstraints.RemoveToken))
            {
                return null;
            }

            if (property.Contains('.'))
            {
                var splits = property.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length != 2)
                {
                    avaloniaObjectType.WriteError($"Can not recognize '{property}'. Skip it.");
                    return null;
                }

                var declaredTypeName = splits[0];
                property = splits[1];
                var manager = _builder.TypeResolver;
                if (manager.TryGetType(declaredTypeName, out var type) == false)
                {
                    return null;
                }
                avaloniaObjectType = type!;
            }

            var field = avaloniaObjectType.GetField($"{property}Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (field == null)
            {
                avaloniaObjectType.WriteError($"Can not find '{property}Property' from '{avaloniaObjectType}'. Skip it.");
            }
            var avaloniaProperty = field?.GetValue(avaloniaObjectType) as AvaloniaProperty;
            return avaloniaProperty;
        }

        public AvaloniaProperty? ParseAcssBehaviorProperty(Type avaloniaObjectType, string property, string? rawValue, out AcssBehavior? value)
        {
            if (rawValue == null)
            {
                value = null;
                return null;
            }
            var declarerName = property[1..];
            if (_builder.BehaviorDeclarerManager.TryGetBehaviorDeclarer(declarerName, out var declarerType) == false)
            {
                value = null;
                return null;
            }

            if (_builder.BehaviorResolverManager.TryGetType(rawValue, out var behaviorType) == false)
            {
                value = null;
                return null;
            }

            value = property.StartsWith(BehaviorConstraints.AddToken)
                ? Activator.CreateInstance(behaviorType!) as AcssBehavior
                : null;

            return _builder.Interpreter.ParseAvaloniaProperty(declarerType!, behaviorType!.Name);
        }

        public object? ParseClrValue(Type declaredType, string? rawValue)
        {
            rawValue = rawValue?.Trim('\'');
            
            // Null.
            if (rawValue is null or "null" or "NULL")
            {
                this.WriteLine($"Raw value is null.");
                return null;
            }

            // Nullable<T>.
            if (declaredType.IsGenericType && declaredType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                declaredType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
            }
            
            // Enum.
            if (declaredType.IsAssignableTo(typeof(Enum)))
            {
                if (Enum.TryParse(declaredType, rawValue, true, out var value))
                {
                    return value;
                }
                else
                {
                    this.WriteError($"Can not parse the value for enum type '{declaredType}'.");
                    return null;
                }
            }

            // String.
            if (declaredType == typeof(string))
            {
                return rawValue;
            }

            // Static instance
            var match = _staticInstanceRegex.Match(rawValue);
            if (match.Success)
            {
                // TODO 支持多级静态实例
                var className = match.Groups[1].Value;
                var instanceName = match.Groups[2].Value;
                if (_builder.TypeResolver.TryGetType(className, out var classType) && classType != null)
                {
                    try
                    {
                        var property = classType.GetProperty(instanceName, BindingFlags.Static | BindingFlags.Public);
                        if (property != null)
                        {
                            var value = property.GetValue(classType);
                            return value;
                        }
                    }
                    catch
                    {
                        // ignored
                    }

                    try
                    {
                        var field = classType.GetField(instanceName, BindingFlags.Static | BindingFlags.Public);
                        if (field != null)
                        {
                            var value = field.GetValue(classType);
                            return value;
                        }
                    }
                    catch
                    {
                        // ignored
                    }
                }

                this.WriteError($"Can not recognize the static instance of '{rawValue}'.");

                return null;
            }

            // Binding
            if (IsBinding(rawValue, out var binding))
            {
                return binding;
            }

            // Adapted parser.
            if (_builder.ValueParsingTypeAdapter.TryAdaptType(declaredType, out var parseType) && parseType != null)
            {
                declaredType = parseType;
            }
            
            // TODO Cache type metadata.

            // Parser.
            var parserMethod = declaredType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new Type[] { typeof(string) });
            if (parserMethod != null)
            {
                return parserMethod.Invoke(declaredType, new object?[] { rawValue });
            }

            // Internal parser.
            var internalParserMethod = declaredType.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new Type[] { typeof(IAcssBuilder), typeof(string) });
            if (internalParserMethod != null)
            {
                return internalParserMethod.Invoke(declaredType, new object?[] { _builder, rawValue });
            }

            declaredType.WriteError($"Can not parse the value '{rawValue}'. Skip it.");
            return null;
        }

        public object? ParseValue(AvaloniaProperty avaloniaProperty, string? rawValue)
        {
            var declareType = avaloniaProperty.PropertyType;

            return ParseClrValue(declareType, rawValue);
        }
        
        public object? ParseDynamicValue(AvaloniaProperty avaloniaProperty, string? rawValue)
        {
            var declareType = avaloniaProperty.PropertyType;
            rawValue = rawValue?.Trim('\'');

            // Var resource.
            if (IsVar(rawValue, out var key))
            {
                var extension = new DynamicResourceExtension(key!);
                return extension;
            }
            
            return ParseClrValue(declareType, rawValue);
        }

        public object? ParseStaticValue(AvaloniaProperty avaloniaProperty, string? rawValue)
        {
            var declareType = avaloniaProperty.PropertyType;
            rawValue = rawValue?.Trim('\'');
            
            // Var resource.
            if (IsVar(rawValue, out var key) && _builder.ResourceProvidersManager.TryFindResource(key!, out var result))
            {
                return result;
            }
            
            return ParseClrValue(declareType, rawValue);
        }

        public bool IsVar(string? valueString, out string? varKey)
        {
            if (valueString == null)
            {
                varKey = null;
                return false;
            }
            var match = _varRegex.Match(valueString);
            if (match.Success)
            {
                varKey = match.Groups[1].Value;
                return true;
            }

            varKey = null;
            return false;
        }

        public bool IsBinding(string? valueString, out Binding? binding)
        {
            binding = null;

            if (valueString == null)
            {
                return false;
            }
            var match = _bindingRegex.Match(valueString);
            if (match.Success)
            {
                var className = match.Groups[1].Value;
                var indexString = match.Groups[2].Value;
                var path = match.Groups[3].Value;

                if (_builder.TypeResolver.TryGetType(className, out var classType) == false)
                {
                    return false;
                }

                if (int.TryParse(indexString, out var index) == false)
                {
                    index = 1;
                }

                binding = new Binding()
                {
                    RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor)
                    {
                        AncestorType = classType,
                        AncestorLevel = index,
                    },
                    Path = path,
                };

                return true;
            }

            return false;
        }

        public ITransition? ParseTransition(string valueString, IResourceHost host)
        {
            var match = _transitionRegex.Match(valueString);
            if (match.Success == false)
            {
                this.WriteError($"Can not parse transition from string '{valueString}'. Skip it.");
                return null;
            }

            // Type name.
            var typeName = match.Groups[1].Value;
            if(_transitionsTypes.TryGetValue(typeName, out var type) == false)
            {
                this.WriteError($"Can not recognize the transition type '{typeName}'. Skip it.");
                return null;
            }
            if (Activator.CreateInstance(type) is not ITransition instance)
            {
                this.WriteError($"The type '{typeName}' is not an implementation of '{nameof(ITransition)}'. Skip it.");
                return null;
            }

            // Value : [target type] [property] [duration] [delay] [easing].
            var targetType   = typeof(TemplatedControl);
            var property     = string.Empty;
            var valuesString = match.Groups[2].Value;
            var values       = valuesString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (values.Length > 0)
            {
                var propertyString = values[0];
                var dotIndex = propertyString.IndexOf('.');
                if (dotIndex >= 0)
                {
                    var ownerTypeName = propertyString[..dotIndex];
                    if (_builder.TypeResolver.TryGetType(ownerTypeName, out var t))
                    {
                        targetType = t;
                    }
                    else
                    {
                        this.WriteError($"Can not recognize the type '{ownerTypeName}'. Use {nameof(TemplatedControl)} as default.");
                    }
                    property = propertyString[++dotIndex..];
                }
                else
                {
                    property = propertyString;
                }
            }
            if (values.Length > 1)
            {
                if (IsVar(values[1], out var keyDuration) && keyDuration != null)
                {
                    host.GetResourceObservable(keyDuration).Subscribe(new AnonymousObserver<object?>(o =>
                    {
                        switch (o)
                        {
                            case double d:
                                instance.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public)
                                        ?.SetValue(instance, TimeSpan.FromSeconds(d));
                                break;
                            case TimeSpan t:
                                instance.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public)
                                        ?.SetValue(instance, t);
                                break;
                        }
                    }));
                }
                else
                {
                    var duration = values[1].TryParseTimeSpan();
                    if (duration != null)
                    {
                        instance.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public)
                                ?.SetValue(instance, duration);
                    }
                }
            }
            if (values.Length > 2)
            {
                if (IsVar(values[2], out var keyDelay) && keyDelay != null)
                {
                    host.GetResourceObservable(keyDelay).Subscribe(new AnonymousObserver<object?>(o =>
                    {
                        switch (o)
                        {
                            case double d:
                                instance.GetType().GetProperty("Delay", BindingFlags.Instance | BindingFlags.Public)
                                        ?.SetValue(instance, TimeSpan.FromSeconds(d));
                                break;
                            case TimeSpan t:
                                instance.GetType().GetProperty("Delay", BindingFlags.Instance | BindingFlags.Public)
                                        ?.SetValue(instance, t);
                                break;
                        }
                    }));
                }
                else
                {
                    var delay = values[2].TryParseTimeSpan();
                    if (delay != null)
                    {
                        instance.GetType().GetProperty("Delay", BindingFlags.Instance | BindingFlags.Public)
                                ?.SetValue(instance, delay);
                    }
                }
            }
            if (values.Length > 3)
            {
                if (IsVar(values[3], out var keyEasing) && keyEasing != null)
                {
                    host.GetResourceObservable(keyEasing).Subscribe(new AnonymousObserver<object?>(o =>
                    {
                        instance.GetType().GetProperty("Easing", BindingFlags.Instance | BindingFlags.Public)
                                ?.SetValue(instance, o);
                    }));
                }
                else
                {
                    var easing = values[3].TryParseEasing();
                    if (easing != null)
                    {
                        instance.GetType().GetProperty("Easing", BindingFlags.Instance | BindingFlags.Public)
                                ?.SetValue(instance, easing);
                    }
                }
            }

            var avaloniaProperty = _builder.Interpreter.ParseAvaloniaProperty(targetType!, property);
            if (avaloniaProperty == null)
            {
                this.WriteError($"Can not recognize the property '{property}' from type '{targetType}'. Skip it.");
                return null;
            }
            
            // TODO Cache property info and method info.
            var propertyProp = instance.GetType().GetProperty("Property", BindingFlags.Instance | BindingFlags.Public);
            propertyProp?.SetValue(instance, avaloniaProperty);

            return instance;
        }

        public IEnumerable<KeyFrame>? ParseKeyFrames(Type selectorTargetType, string valueString)
        {
            valueString = valueString[1..^1].Trim();
            var parser      = _builder.Parser;
            var interpreter = _builder.Interpreter;
            var objects     = parser.ParseCollectionObjects(valueString);

            foreach (var (selector, propertySettingsString) in objects)
            {
                var match = _keyFrameRegex.Match(selector);
                if (!match.Success)
                {
                    continue;
                }

                // Initial.
                var keyFrame = new KeyFrame();
                var initString = match.Groups[1].Value;
                var splits = initString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                // Cue or KeyTime.
                if (splits.Length > 0)
                {
                    try
                    {
                        if (splits[0].EndsWith("%"))
                        {
                            keyFrame.Cue = Cue.Parse(splits[0], CultureInfo.InvariantCulture);
                        }
                        else
                        {
                            keyFrame.KeyTime = splits[0].TryParseTimeSpan() ?? TimeSpan.Zero;
                        }
                    }
                    catch
                    {
                        this.WriteError($"Can not parse Cue or KeyTime from string '{splits[0]}' in '{selector}'. Use default value.");
                    }
                }
                // KeySpine
                if (splits.Length > 1 && string.IsNullOrEmpty(splits[1]) == false)
                {
                    try
                    {
                        keyFrame.KeySpline = splits[1].TryParseKeySpline();
                    }
                    catch (Exception e)
                    {
                        this.WriteError($"Invalid key spline '{splits[0]}'. Skip it.");
                    }
                }

                // Setters.
                var pairs = parser.ParsePairs(propertySettingsString);
                foreach (var pair in pairs)
                {
                    var propertyName = pair.Item1;
                    var matchAnimator = _setterAnimatorRegex.Match(propertyName);
                    string? animatorTypeName = null;
                    if (matchAnimator.Success)
                    {
                        propertyName = matchAnimator.Groups[1].Value;
                        animatorTypeName = matchAnimator.Groups[2].Value;
                    }

                    var setter = new Setter();

                    // Property
                    var property = interpreter.ParseAvaloniaProperty(selectorTargetType, propertyName);
                    if (property == null)
                    {
                        continue;
                    }
                    setter.Property = property;

                    // Value
                    if (IsVar(pair.Item2, out var key))
                    {
                        // Dynamic.
                        var app = Checks.CheckApplication();
                        app.GetResourceObservable(key!).Subscribe(new AnonymousObserver<object?>(o =>
                        {
                            setter.Value = o;
                            keyFrame.Setters.Remove(setter);
                            keyFrame.Setters.Add(setter);
                        }));
                    }
                    else
                    {
                        // Static.
                        setter.Value = interpreter.ParseValue(property, pair.Item2);
                        keyFrame.Setters.Add(setter);
                    }

                    if (animatorTypeName != null && _builder.TypeResolver.TryGetType(animatorTypeName, out var animatorType))
                    {
                        var animator = Activator.CreateInstance(animatorType!);
                        if (animator is ICustomAnimator customAnimator)
                        {
                            Animation.SetAnimator(setter, customAnimator);
                        }
                        else
                        {
                            this.WriteError($"The type '{animatorType}' is not a {nameof(ICustomAnimator)}. It can not be an animator for an animation.");
                        }
                    }
                }

                yield return keyFrame;
            }
        }

        public LinearGradientBrush? ParseLinear(string valueString, IResourceHost host)
        {
            var match = _linearRegex.Match(valueString.ReplaceLineEndings(string.Empty));
            if (!match.Success)
            {
                this.WriteError($"Can not parse liner brush from string '{valueString}'. Skip it.");
                return null;
            }

            var d1Str = match.Groups[1].Value;
            var d2Str = match.Groups[2].Value;
            var d3Str = match.Groups[3].Value;
            var d4Str = match.Groups[4].Value;
            var contentStr = match.Groups[5].Value;

            var linearBrush = new LinearGradientBrush();

            // Initial.
            try
            {
                linearBrush.StartPoint = RelativePoint.Parse($"{d1Str},{d2Str}");
                linearBrush.EndPoint = RelativePoint.Parse($"{d3Str},{d4Str}");
            }
            catch (Exception e)
            {
                this.WriteError($"Can not parse liner brush from string '{valueString}' because of {e}.");
                return null;
            }
            
            // Setters.
            var stopList = contentStr.Trim().Split(';', StringSplitOptions.RemoveEmptyEntries);
            foreach (var stopString in stopList)
            {
                var variables = stopString.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (variables.Length is < 2 or > 3)
                {
                    this.WriteError($"Invalid gradient stop value '{stopString}'. Skip it.");
                    continue;
                }

                // Check var.
                var isDyn = _builder.Interpreter.IsVar(variables[0], out var key);
                
                // Offset.
                var opacityString = (string?)null;
                var offsetString = variables[1];
                if (variables.Length == 3)
                {
                    opacityString = variables[1];
                    offsetString = variables[2];
                }
                if (double.TryParse(offsetString, out var offset) == false)
                {
                    this.WriteError($"Invalid gradient offset value '{offsetString}'. Skip it.");
                    continue;
                }
                
                // Opacity
                var existOpacity = double.TryParse(opacityString, out var opacity);

                var stop = new GradientStop
                {
                    Offset = offset,
                };

                if (isDyn == false)
                {
                    // Color
                    var color = variables[0].TryParseColor();
                    if (color == null)
                    {
                        this.WriteError($"Invalid gradient stop color value '{variables[0]}'. Skip it.");
                        continue;
                    }
                    
                    if (existOpacity)
                    {
                        color = color.Value.ApplyOpacity(opacity);
                    }

                    stop.Color = color.Value;
                }
                else
                {
                    host.GetResourceObservable(key!).Subscribe(new AnonymousObserver<object?>(o =>
                    {
                        if (o is not Color c)
                        {
                            return;
                        }
                        if (existOpacity)
                        {
                            c = c.ApplyOpacity(opacity);
                        }

                        stop.Color = c;
                    }));
                }

                linearBrush.GradientStops.Add(stop);
            }
            
            return linearBrush;
        }

        public LinearGradientBrush? ParseComplexLinear(string valueString, IResourceHost host)
        {
            return null;
        }

        public string ParseSelectorAndBases(string header, out IList<string>? bases)
        {
            var match = _basesRegex.Match(header.ReplaceLineEndings(string.Empty));
            if (match.Success)
            {
                var selector = match.Groups[1].Value;
                var content = match.Groups[2].Value;
                bases = content.Split(new char[] { ',', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                return selector;
            }

            bases = null;
            return header;
        }
    }
}