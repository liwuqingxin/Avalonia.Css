using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css;

public static class CssResourceFactory
{
    private static readonly Dictionary<string, IResourceFactory> Factories = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Regex Regex = BuildRegex();

    private static Regex BuildRegex()
    {
        var builder = new StringBuilder();
        builder.Append('(');
        builder.AppendJoin('|', Enum.GetNames(typeof(ResourceTypes)));
        builder.Append(")\\s*\\(([a-zA-Z0-9_\\-\\.]*?)\\)\\s*\\:\\s*(.*)[;]?");

        return new Regex(builder.ToString(), RegexOptions.IgnoreCase);
    }

    static CssResourceFactory()
    {
        var factories = typeof(CssResourceFactory).Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IResourceFactory)) && t.IsAbstract == false)
            .Select(Activator.CreateInstance)
            .OfType<IResourceFactory>();

        foreach (var factory in factories)
        {
            Factories[factory.GetType().Name.Replace("Resource", "")] = factory;
        }
    }

    public static bool TryGetResourceInstance(string resourceString, out CssResource? resource)
    {
        var match = Regex.Match(resourceString);
        if (match.Success == false)
        {
            resource = null;
            return false;
        }

        var type        = match.Groups[1].Value;
        var key         = match.Groups[2].Value;
        var valueString = match.Groups[3].Value.Trim().TrimEnd(';');

        if (Factories.TryGetValue(type, out var factory))
        {
            resource = factory.Create();
            resource.AcceptCore(key, valueString);
            return true;
        }

        resource = null;
        return false;
    }
}
