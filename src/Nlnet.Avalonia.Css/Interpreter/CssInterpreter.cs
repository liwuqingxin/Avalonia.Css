using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Styling;
using Avalonia;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using System;
using System.Globalization;
using System.Linq;
using Avalonia.Data;

namespace Nlnet.Avalonia.Css
{
    public interface ICssInterpreter
    {
        public Selector? ToSelector(IEnumerable<ISyntax> syntaxList);

        public AvaloniaProperty? ParseAvaloniaProperty(Type avaloniaObjectType, string property);

        public object? ParseValue(Type declaredType, string? rawValue);

        public object? ParseValue(AvaloniaProperty avaloniaProperty, string? rawValue);

        public bool IsVar(string? valueString, out string? varKey);

        public bool IsBinding(string? valueString, out Binding? binding);

        public ITransition? ParseTransition(string valueString);

        public IEnumerable<KeyFrame>? ParseKeyFrames(Type selectorTargetType, string valueString);
    }

    public class CssInterpreter : ICssInterpreter
    {
        private readonly Regex             _varRegex            = new("^\\s*var\\s*\\((.*?)\\)\\s*$", RegexOptions.IgnoreCase);
        private readonly Regex             _bindingRegex        = new("^\\s*\\$([a-zA-Z0-9_]+)#?([0-9]*)\\.(.*?)\\s*$", RegexOptions.IgnoreCase);
        private readonly Regex             _staticInstanceRegex = new("^\\s*@([a-zA-Z0-9_]+)\\.([a-zA-Z0-9_]+)\\s*$", RegexOptions.IgnoreCase);
        private readonly Regex             _transitionRegex     = new("([a-zA-Z]+)\\((.*)\\)", RegexOptions.IgnoreCase);
        private readonly Regex             _keyFrameRegex       = new("^\\s*KeyFrame\\s*\\((.*?)\\)\\s*\\:\\s*$", RegexOptions.IgnoreCase);
        private readonly IEnumerable<Type> _transitionsTypes;

        public CssInterpreter()
        {
            _transitionsTypes = typeof(Transition<>).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ITransition)) && t.IsAbstract == false);
        }

        public Selector? ToSelector(IEnumerable<ISyntax> syntaxList)
        {
            return syntaxList.Aggregate<ISyntax, Selector?>(null, (current, syntax) => syntax.ToSelector(current));
        }

        public AvaloniaProperty? ParseAvaloniaProperty(Type avaloniaObjectType, string property)
        {
            if (property.Contains('.'))
            {
                var splits = property.Split('.', StringSplitOptions.RemoveEmptyEntries);
                if (splits.Length != 2)
                {
                    avaloniaObjectType.WriteLine($"Can not recognize '{property}'. Skip it.");
                    return null;
                }

                var declaredTypeName = splits[0];
                property = splits[1];
                var manager = ServiceLocator.GetService<ITypeResolverManager>();
                if (manager.TryGetType(declaredTypeName, out var type) == false)
                {
                    avaloniaObjectType.WriteLine($"Can not find '{declaredTypeName}' from '{nameof(TypeResolverManager)}'. Skip it.");
                    return null;
                }
                avaloniaObjectType = type!;
            }

            var field = avaloniaObjectType.GetField($"{property}Property", BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
            if (field == null)
            {
                avaloniaObjectType.WriteLine($"Can not find '{property}Property' from '{avaloniaObjectType}'. Skip it.");
            }
            var avaloniaProperty = field?.GetValue(avaloniaObjectType) as AvaloniaProperty;
            return avaloniaProperty;
        }

        public object? ParseValue(Type declaredType, string? rawValue)
        {
            rawValue = rawValue?.Trim('\'');
            if (rawValue is null or "null" or "NULL")
            {
                declaredType.WriteLine($"Raw value is null.");
                return null;
            }

            // Resource.
            if (IsVar(rawValue, out var key))
            {
                var extension = new DynamicResourceExtension(key!);
                return extension;
            }

            // Enum.
            if (declaredType.IsAssignableTo(typeof(Enum)))
            {
                return Enum.TryParse(declaredType, rawValue, true, out var value) ? value : null;
            }

            // String.
            if (declaredType == typeof(string))
            {
                return rawValue;
            }

            var match   = _staticInstanceRegex.Match(rawValue);
            var manager = ServiceLocator.GetService<ITypeResolverManager>();
            if (match.Success)
            {
                var className = match.Groups[1].Value;
                var instanceName = match.Groups[2].Value;
                if (manager.TryGetType(className, out var classType) && classType != null)
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

                return null;
            }

            // Binding
            if (IsBinding(rawValue, out var binding))
            {
                return binding;
            }

            // Adapted parser.
            if (manager.TryAdaptType(declaredType, out var parseType) && parseType != null)
            {
                declaredType = parseType;
            }

            // Parser.
            var parserMethod = declaredType!.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new Type[] { typeof(string) });
            if (parserMethod == null)
            {
                declaredType.WriteLine($"Can not find the 'Parse' method in '{declaredType}'. Skip it.");
                return null;
            }
            return parserMethod.Invoke(declaredType, new object?[] { rawValue });
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

                if (ServiceLocator.GetService<ITypeResolverManager>().TryGetType(className, out var classType) == false)
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
                return null;
            }

            var typeName = match.Groups[1].Value;
            var valuesString = match.Groups[2].Value;
            var type = _transitionsTypes.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase));
            if (type == null)
            {
                return null;
            }

            if (Activator.CreateInstance(type) is not ITransition instance)
            {
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
                    var manager = ServiceLocator.GetService<ITypeResolverManager>();
                    if (manager.TryGetType(propertyString[..dotIndex], out var t))
                    {
                        targetType = t;
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
                duration = DataParser.ParseTimeSpan(values[1]);
            }
            if (values.Length > 2)
            {
                delay = DataParser.ParseTimeSpan(values[2]);
            }
            if (values.Length > 3)
            {
                try
                {
                    easing = Easing.Parse(values[3]);
                }
                catch (Exception e)
                {
                    // ignore
                }
            }

            var avaloniaProperty = ServiceLocator.GetService<ICssInterpreter>().ParseAvaloniaProperty(targetType!, property);
            if (avaloniaProperty == null)
            {
                return null;
            }

            instance.Property = avaloniaProperty;
            var durationProp = instance.GetType().GetProperty("Duration", BindingFlags.Instance | BindingFlags.Public);
            var delayProp = instance.GetType().GetProperty("Delay", BindingFlags.Instance | BindingFlags.Public);
            var easingProp = instance.GetType().GetProperty("Easing", BindingFlags.Instance | BindingFlags.Public);
            durationProp?.SetValue(instance, duration);
            delayProp?.SetValue(instance, delay);
            easingProp?.SetValue(instance, easing);

            return instance;
        }

        public IEnumerable<KeyFrame>? ParseKeyFrames(Type selectorTargetType, string valueString)
        {
            valueString = valueString[1..^1].Trim(' ');
            var parser  = ServiceLocator.GetService<ICssParser>();
            var interpreter = ServiceLocator.GetService<ICssInterpreter>();
            var objects = parser.ParseObjects(valueString);

            foreach (var (selector, propertySettingsString) in objects)
            {
                var match = _keyFrameRegex.Match(selector);
                if (!match.Success)
                {
                    continue;
                }

                // Initial.
                var keyFrame   = new KeyFrame();
                var initString = match.Groups[1].Value;
                var splits     = initString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
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
                            keyFrame.KeyTime = TimeSpan.Parse(splits[0]);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }
                if (splits.Length > 1 && string.IsNullOrEmpty(splits[1]) == false)
                {
                    try
                    {
                        var keySpline = KeySpline.Parse(splits[1], CultureInfo.InvariantCulture);
                        keyFrame.KeySpline = keySpline;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                }

                // Setters.
                var pairs = parser.ParsePairs(propertySettingsString);
                foreach (var pair in pairs)
                {
                    var property = interpreter.ParseAvaloniaProperty(selectorTargetType, pair.Item1);
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
                    keyFrame.Setters.Add(setter);
                }

                yield return keyFrame;
            }
        }
    }
}