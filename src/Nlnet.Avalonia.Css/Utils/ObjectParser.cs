using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nlnet.Avalonia.Css;

internal static class ObjectParser
{
    public static void ApplySetters(this object targetObject, Type type, IAcssInterpreter interpreter, IEnumerable<(string, string)> setters, params string[] excepts)
    {
        foreach (var setter in setters.Where(s => excepts.Contains(s.Item1) == false))
        {
            var property = type.GetProperty(setter.Item1);
            if (property == null)
            {
                typeof(ObjectParser).WriteError($"Can not get the property '{setter.Item1}' from type of '{type}'. Skip it.");
                continue;
            }

            var value = interpreter.ParseValue(property.PropertyType, setter.Item2);
            if (property.CanWrite)
            {
                property.SetValue(targetObject, value);
            }
            else
            {
                // We assume that the property is a List<T> instance.
                
                if (value == null)
                {
                    typeof(ObjectParser).WriteError($"The property '{property}' is readonly and the target value is null. Skip it.");
                    return;
                }
                
                var origin = property.GetValue(targetObject);
                if (origin == null)
                {
                    typeof(ObjectParser).WriteError($"The property '{property}' is readonly and the origin value is null. Skip it.");
                    return;
                }
                var method = origin.GetType().GetMethod("AddRange", BindingFlags.Public | BindingFlags.Instance);
                if (method == null)
                {
                    typeof(ObjectParser).WriteError($"The property '{property}' is readonly and we can not get the AddRange method from the origin value object. Skip it.");
                    return;
                }

                try
                {
                    method.Invoke(origin, new object[] { value });
                }
                catch (Exception e)
                {
                    typeof(ObjectParser).WriteError(e.ToString());
                }
            }
        }
    }
    
    public static void ApplySetters(this object targetObject, IAcssInterpreter interpreter, IEnumerable<(string, string)> setters, params string[] excepts)
    {
        var type = targetObject.GetType();
        ApplySetters(targetObject, type, interpreter, setters, excepts);
    }
}