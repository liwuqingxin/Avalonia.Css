using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css;

internal interface ICssResourceFactory
{
    public bool TryGetResourceInstance(string resourceString, out CssResource? resource);
}

internal class CssResourceFactory : ICssResourceFactory
{
    private readonly ICssBuilder _builder;

    private static readonly Dictionary<string, IResourceFactory> Factories = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Regex Regex;

    static CssResourceFactory()
    {
        var factories = typeof(CssResourceFactory).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IResourceFactory)) && t.IsAbstract == false)
            .Select(t =>
            {
                var factory = Activator.CreateInstance(t) as IResourceFactory;
                var keys    = t.GetCustomAttributes<ResourceTypeAttribute>();
                return (keys, factory);
            })
            .SelectMany(t =>
            {
                return t.keys.Select(key => (key.Type, t.factory));
            })
            .Where(t => t.factory != null)
            .ToList();

        foreach (var factory in factories)
        {
            Factories[factory.Type] = factory.factory!;
            DiagnosisHelper.WriteLine($"Setup resource factory '{factory.factory}' for type '{factory.Type}'.");
        }

        var builder = new StringBuilder();
        builder.Append('(');
        builder.AppendJoin('|', factories.Select(t => t.Type));
        builder.Append(")\\s*\\(([a-zA-Z0-9_\\-\\.]*?)\\)\\s*\\:\\s*(.*)[;]?");

        Regex = new Regex(builder.ToString(), RegexOptions.IgnoreCase);
    }

    public CssResourceFactory(ICssBuilder builder)
    {
        _builder = builder;
    }

    public bool TryGetResourceInstance(string resourceString, out CssResource? resource)
    {
        var match = Regex.Match(resourceString);
        if (match.Success == false)
        {
            this.WriteError($"Resource '{resourceString}' is invalid. Skip it.");
            resource = null;
            return false;
        }

        var type        = match.Groups[1].Value;
        var key         = match.Groups[2].Value;
        var valueString = match.Groups[3].Value.Trim().TrimEnd(';');

        if (Factories.TryGetValue(type, out var factory))
        {
            resource = factory.Create();
            resource.AcceptCore(_builder, key, valueString);
            return true;
        }

        this.WriteError($"Resource type '{type}' is not supported now.");
        resource = null;
        return false;
    }
}
