using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Avalonia;

namespace Nlnet.Avalonia.Css;

public interface IBehaviorResolver : IResolver
{
    
}

public class GenericBehaviorResolver<TTypeSink> : Resolver, IBehaviorResolver
{
    public GenericBehaviorResolver()
    {
        var typeSink = typeof(TTypeSink);
        var assembly = typeSink.Assembly;
        var types = assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(AcssBehavior)) && t.IsAbstract == false)
            .Select(t => (attr: t.GetCustomAttribute<BehaviorAttribute>(), facType: t))
            .Where(tuple => tuple.attr != null)
            .ToDictionary(tuple => tuple.attr!.Name, tuple => tuple.facType);

        Types = types;

        foreach (var type in Types)
        {
            this.WriteLine($"Map behavior '{type.Key}' to '{type.Value}'");
        }
    }
}