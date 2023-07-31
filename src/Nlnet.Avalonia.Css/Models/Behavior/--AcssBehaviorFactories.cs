//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;

//namespace Nlnet.Avalonia.Css;

//public static class AcssBehaviorFactories
//{
//    private static readonly Dictionary<string, AcssBehavior> Factories;

//    static AcssBehaviorFactories()
//    {
//        var factories = typeof(AcssBehaviorFactories).Assembly.GetTypes()
//            .Where(t => t.IsAssignableTo(typeof(AcssBehavior<>)) && t.IsAbstract == false)
//            .Select(t => (attr: t.GetCustomAttribute<BehaviorAttribute>(), facType: t))
//            .Where(tuple => tuple.attr != null)
//            .Select(tuple => (tuple.attr, fac: (AcssBehavior)Activator.CreateInstance(tuple.facType)!))
//            .ToDictionary(tuple => tuple.attr!.Name, tuple =>
//            {
//                tuple.fac.TargetType = tuple.attr?.TargetType;
//                return tuple.fac;
//            });

//        Factories = new Dictionary<string, AcssBehavior>(factories, StringComparer.OrdinalIgnoreCase);
//    }

//    public static bool TryGetBehavior(string key, out AcssBehavior? behavior)
//    {
//        if (Factories.TryGetValue(key, out var zero))
//        {
//            behavior = zero.Get();
//            return true;
//        }

//        behavior = null;
//        return false;
//    }
//}
