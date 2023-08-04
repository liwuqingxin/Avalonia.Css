using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;

namespace Nlnet.Avalonia.Css
{
    internal class AcssInterpreter : IAcssInterpreter
    {
        private readonly IAcssBuilder _builder;

        // ' var (xxx) '
        private readonly Regex _varRegex = new("^\\s*var\\s*\\((.*?)\\)\\s*$", RegexOptions.IgnoreCase);
        // ' $(xxx).xxx ' or '$(xxx)#10.xxx '
        private readonly Regex _bindingRegex = new("^\\s*\\$([a-zA-Z0-9_]+)#?([0-9]*)\\.(.*?)\\s*$", RegexOptions.IgnoreCase);
        // ' @xxx.xxx '
        private readonly Regex _staticInstanceRegex = new("^\\s*@([a-zA-Z0-9_]+)\\.([a-zA-Z0-9_]+)\\s*$", RegexOptions.IgnoreCase);
        // ' xxx(xxx) '
        private readonly Regex _transitionRegex = new("([a-zA-Z]+)\\((.*)\\)", RegexOptions.IgnoreCase);
        // ' KeyFrame (xxx) : '
        private readonly Regex _keyFrameRegex = new("^\\s*KeyFrame\\s*\\((.*?)\\)\\s*\\:\\s*$", RegexOptions.IgnoreCase);
        // ' xxx (xxx) '
        private readonly Regex _setterAnimatorRegex = new("\\s*(.*?)\\s*\\(([a-zA-Z0-9_]*)\\)\\s*");

        private readonly IEnumerable<Type> _transitionsTypes;

        public AcssInterpreter(IAcssBuilder builder)
        {
            _builder = builder;
            _transitionsTypes = typeof(Transition<>).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ITransition)) && t.IsAbstract == false);
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

        public object? ParseValue(Type declaredType, string? rawValue)
        {
            if (declaredType.IsGenericType && declaredType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                declaredType = Nullable.GetUnderlyingType(declaredType) ?? declaredType;
            }

            rawValue = rawValue?.Trim('\'');
            if (rawValue is null or "null" or "NULL")
            {
                declaredType.WriteLine($"Raw value is null.");
                return null;
            }

            // Var resource.
            if (IsVar(rawValue, out var key))
            {
                var extension = new DynamicResourceExtension(key!);
                return extension;
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

            return ParseValue(declareType, rawValue);
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

        public ITransition? ParseTransition(string valueString)
        {
            var match = _transitionRegex.Match(valueString);
            if (match.Success == false)
            {
                this.WriteError($"Can not parse transition from string '{valueString}'. Skip it.");
                return null;
            }

            var typeName = match.Groups[1].Value;
            var valuesString = match.Groups[2].Value;
            var type = _transitionsTypes.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase));
            if (type == null)
            {
                this.WriteError($"Can not recognize the transition type '{typeName}'. Skip it.");
                return null;
            }

            if (Activator.CreateInstance(type) is not ITransition instance)
            {
                this.WriteError($"The type '{typeName}' is not an implementation of '{nameof(ITransition)}'. Skip it.");
                return null;
            }

            var targetType = typeof(TemplatedControl);
            var property = string.Empty;
            var duration = new TimeSpan(0, 0, 0, 0);
            var delay = new TimeSpan(0, 0, 0, 0);
            var easing = (Easing?)new LinearEasing();

            var values = valuesString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (values.Length > 0)
            {
                var propertyString = values[0];
                var dotIndex = propertyString.IndexOf('.');
                if (dotIndex >= 0)
                {
                    var manager       = _builder.TypeResolver;
                    var ownerTypeName = propertyString[..dotIndex];
                    if (manager.TryGetType(ownerTypeName, out var t))
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
                duration = DataParser.TryParseTimeSpan(values[1]);
            }
            if (values.Length > 2)
            {
                delay = DataParser.TryParseTimeSpan(values[2]);
            }
            if (values.Length > 3)
            {
                easing = DataParser.TryParseEasing(values[3]);
            }

            var avaloniaProperty = _builder.Interpreter.ParseAvaloniaProperty(targetType!, property);
            if (avaloniaProperty == null)
            {
                return null;
            }
            
            var propertyProp = instance.GetType().GetProperty("Property", BindingFlags.Instance | BindingFlags.Public);
            var durationProp = instance.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public);
            var delayProp    = instance.GetType().GetProperty("Delay",    BindingFlags.Instance | BindingFlags.Public);
            var easingProp   = instance.GetType().GetProperty("Easing",   BindingFlags.Instance | BindingFlags.Public);
            propertyProp?.SetValue(instance, avaloniaProperty);
            durationProp?.SetValue(instance, duration);
            delayProp?.SetValue(instance, delay);
            easingProp?.SetValue(instance, easing);

            return instance;
        }

        public IEnumerable<KeyFrame>? ParseKeyFrames(Type selectorTargetType, string valueString)
        {
            valueString = valueString[1..^1].Trim();
            var parser      = _builder.Parser;
            var interpreter = _builder.Interpreter;
            var objects     = parser.ParseObjects(valueString);

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
                            keyFrame.KeyTime = DataParser.TryParseTimeSpan(splits[0]);
                        }
                    }
                    catch
                    {
                        this.WriteError($"Can not parse Cue or KeyTime from string '{splits[0]}' in '{selector}'. Use default value.");
                    }
                }
                if (splits.Length > 1 && string.IsNullOrEmpty(splits[1]) == false)
                {
                    try
                    {
                        var keySpline = KeySpline.Parse(splits[1], CultureInfo.InvariantCulture);
                        keyFrame.KeySpline = keySpline;
                    }
                    catch
                    {
                        this.WriteError($"Can not parse KeySpline from string '{splits[1]}' in '{selector}'.");
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

                    var property = interpreter.ParseAvaloniaProperty(selectorTargetType, propertyName);
                    if (property == null)
                    {
                        continue;
                    }
                    var value = interpreter.ParseValue(property, pair.Item2);
                    var setter = new Setter()
                    {
                        Property = property,
                        Value = value
                    };

                    if (animatorTypeName != null)
                    {
                        if (_builder.TypeResolver.TryGetType(animatorTypeName, out var animatorType))
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

                    keyFrame.Setters.Add(setter);
                }

                yield return keyFrame;
            }
        }
    }
}