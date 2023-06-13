using System;
using Avalonia.Styling;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System.Text.RegularExpressions;
using Avalonia.Animation.Easings;
using Avalonia.Animation;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Nlnet.Avalonia.Css
{
    internal static class InterpreterHelper
    {
        private static readonly Regex VarRegex = new("^\\s*var\\((.*?)\\)\\s*$", RegexOptions.IgnoreCase);
        private static readonly Regex TransitionRegex = new("([a-zA-Z]+)\\((.*)\\)", RegexOptions.IgnoreCase);
        private static readonly IEnumerable<Type> TransitionsTypes;

        static InterpreterHelper()
        {
            TransitionsTypes = typeof(Transition<>).Assembly
                .GetTypes()
                .Where(t => t.IsAssignableTo(typeof(ITransition)) && t.IsAbstract == false);
        }

        internal static Selector? ToSelector(IEnumerable<ISyntax> syntaxList)
        {
            return syntaxList.Aggregate<ISyntax, Selector?>(null, (current, syntax) => syntax.ToSelector(current));
        }

        internal static AvaloniaProperty? ParseAvaloniaProperty(Type avaloniaObjectType, string property)
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
                if (TypeResolver.Instance.TryGetType(declaredTypeName, out var type) == false)
                {
                    avaloniaObjectType.WriteLine($"Can not find '{declaredTypeName}' from '{nameof(TypeResolver)}'. Skip it.");
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

        internal static object? ParseValue(AvaloniaProperty avaloniaProperty, string? rawValue)
        {
            if (rawValue == null)
            {
                avaloniaProperty.WriteLine($"Raw value is null. Skip it.");
                return null;
            }

            var declareType = avaloniaProperty.PropertyType;
            if (IsVar(rawValue, out var key))
            {
                var extension = new DynamicResourceExtension(key!);
                return extension;
            }
            if (declareType.IsAssignableTo(typeof(Enum)))
            {
                return Enum.TryParse(declareType, rawValue, true, out var value) ? value : null;
            }
            if (declareType == typeof(string))
            {
                return rawValue;
            }

            if (TypeResolver.Instance.TryAdaptType(declareType, out var parseType))
            {
                declareType = parseType;
            }
            var parserMethod = declareType!.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic, new Type[] { typeof(string) });
            if (parserMethod == null)
            {
                avaloniaProperty.WriteLine($"Can not find the 'Parse' method in '{declareType}'. Skip it.");
                return null;
            }
            return parserMethod.Invoke(declareType, new object?[] { rawValue });
        }

        internal static bool IsVar(string? valueString, out string? varKey)
        {
            if (valueString == null)
            {
                varKey = null;
                return false;
            }
            var match = VarRegex.Match(valueString);
            if (match.Success)
            {
                varKey = match.Groups[1].Value;
                return true;
            }

            varKey = null;
            return false;
        }

        internal static ITransition? ParseTransition(string valueString)
        {
            var match = TransitionRegex.Match(valueString);
            if (match.Success == false)
            {
                return null;
            }

            var typeName = match.Groups[1].Value;
            var valuesString = match.Groups[2].Value;
            var type = TransitionsTypes.FirstOrDefault(t => t.Name.Equals(typeName, StringComparison.CurrentCultureIgnoreCase));
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
            var easing = (Easing?) new LinearEasing();

            var values = valuesString.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (values.Length > 0)
            {
                var propertyString = values[0];
                var dotIndex = propertyString.IndexOf('.');
                if (dotIndex >= 0)
                {
                    if (TypeResolver.Instance.TryGetType(propertyString[..dotIndex], out var t))
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
                duration = ParseTimeSpan(values[1]);
            }
            if (values.Length > 2)
            {
                delay = ParseTimeSpan(values[2]);
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

            var avaloniaProperty = InterpreterHelper.ParseAvaloniaProperty(targetType!, property);
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

        private static TimeSpan ParseTimeSpan(string value)
        {
            if (TimeSpan.TryParse(value, out var duration))
            {
                return duration;
            }

            if (double.TryParse(value, out var milliseconds))
            {
                duration = TimeSpan.FromSeconds(milliseconds);
            }

            return duration;
        }

        internal static IEnumerable<CssSetter> ParseSetters(ReadOnlySpan<char> span)
        {
            var setters = new List<CssSetter>();
            
            var colonsCount = 0;
            var index = 0;
            var name = string.Empty;
            string value;
            var isInObject = false;

            for (var i = 0; i < span.Length; i++)
            {
                switch (span[i])
                {
                    case ':':
                        if (isInObject)
                        {
                            continue;
                        }
                        if (colonsCount == 0)
                        {
                            name = span[index..i].ToString().Trim();
                            index = i + 1;
                        }
                        colonsCount++;
                        break;
                    case ';':
                        if (isInObject)
                        {
                            continue;
                        }
                        value     = span[index..i].ToString().Trim();
                        index     = i + 1;
                        setters.Add(new CssSetter(name, value));
                        name = string.Empty;
                        colonsCount--;
                        break;
                    case '[':
                        isInObject = true;
                        break;
                    case ']':
                        isInObject = false;
                        value      = span.Slice(index, i - index + 1).ToString().Trim();
                        index      = i + 1;
                        setters.Add(new CssSetter(name, value));
                        name = string.Empty;
                        colonsCount--;
                        break;
                }
            }

            if (index < span.Length && string.IsNullOrEmpty(name) == false)
            {
                value     = span[index..].ToString().Trim(';',' ');
                setters.Add(new CssSetter(name, value));
            }

            return setters;
        }
    }
}
