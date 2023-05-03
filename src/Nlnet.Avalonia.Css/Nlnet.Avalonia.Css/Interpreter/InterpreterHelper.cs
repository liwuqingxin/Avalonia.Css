using System;
using Avalonia.Styling;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Avalonia;

namespace Nlnet.Avalonia.Css
{
    internal static class InterpreterHelper
    {
        internal static Selector? ToSelector(IEnumerable<ISyntax> syntaxList)
        {
            return syntaxList.Aggregate<ISyntax, Selector?>(null, (current, syntax) => syntax.ToSelector(current));
        }

        internal static AvaloniaProperty? GetAvaloniaProperty(Type avaloniaObjectType, string property)
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
            if (declareType.IsAssignableTo(typeof(Enum)))
            {
                return Enum.TryParse(declareType, rawValue, true, out var value) ? value : null;
            }
            else
            {
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
        }
    }
}
