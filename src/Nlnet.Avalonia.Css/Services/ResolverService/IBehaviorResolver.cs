using System.Collections.Generic;
using System;
using System.Linq;
using System.Reflection;
using Avalonia;

namespace Nlnet.Avalonia.Css;

/// <summary>
/// Interface that can provide the service of resolving acss behavior from string.
/// </summary>
public interface IBehaviorResolver : IResolver
{
    
}

/// <summary>
/// A generic implementation for <see cref="IBehaviorResolver"/> that can provide acss behavior resolving service for assembly who contains the class of <see cref="TTypeSink"/>.
/// </summary>
/// <typeparam name="TTypeSink"></typeparam>
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