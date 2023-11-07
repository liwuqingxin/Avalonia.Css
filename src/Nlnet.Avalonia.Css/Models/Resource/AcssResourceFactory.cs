using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Nlnet.Avalonia.Css;

internal interface IAcssResourceFactory : IService
{
    public bool TryGetResourceInstance(string def, string value, out AcssResource? resource);
}

// TODO Provide accesses to resource definitions.

internal class AcssResourceFactory : IAcssResourceFactory
{
    private readonly IAcssContext _context;

    private static readonly Dictionary<string, IResourceFactory> Factories = new(StringComparer.OrdinalIgnoreCase);

    private static readonly Regex Regex = new("^\\s*(.*)\\s*\\((.*)\\)", RegexOptions.IgnoreCase);

    static AcssResourceFactory()
    {
        var factories = typeof(AcssResourceFactory).Assembly
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
    }

    public AcssResourceFactory(IAcssContext context)
    {
        _context = context;
    }

    public void Initialize()
    {

    }

    public bool TryGetResourceInstance(string def, string value, out AcssResource? resource)
    {
        var match = Regex.Match(def);
        if (match.Success == false)
        {
            _context.OnError(AcssErrors.Resource_Invalid, $"Resource '{def}' is invalid. Skip it.");
            resource = null;
            return false;
        }

        var type = match.Groups[1].Value;
        var key  = match.Groups[2].Value;

        if (Factories.TryGetValue(type, out var factory))
        {
            resource = factory.Create();
            resource.Accept(key, value);
            return true;
        }

        _context.OnError(AcssErrors.Resource_Not_Supported, $"Resource type '{type}' is not supported now.");
        resource = null;
        return false;
    }
}
