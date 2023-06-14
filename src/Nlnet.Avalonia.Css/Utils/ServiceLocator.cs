using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> Services = new();

        public static void SetService<TInterface, TImplement>()
            where TInterface : class
            where TImplement : class, TInterface, new()
        {
            Services[typeof(TInterface)] = new TImplement();
        }

        public static void SetService<TInterface, TImplement>(TImplement service)
            where TInterface : class
            where TImplement : class, TInterface
        {
            Services[typeof(TInterface)] = service;
        }

        public static TInterface GetService<TInterface>() where TInterface : class
        {
            if (Services.TryGetValue(typeof(TInterface), out var service) && service is TInterface t)
            {
                return t;
            }

            throw new Exception($"Can not find the {typeof(TInterface)} service.");
        }

        static ServiceLocator()
        {
            ServiceLocator.SetService<ITypeResolverManager, TypeResolverManager>();
            ServiceLocator.SetService<ICssParser, CssParser>();
            ServiceLocator.SetService<ICssInterpreter, CssInterpreter>();
            ServiceLocator.SetService<ICssManager, CssManager>();
            ServiceLocator.SetService<ICssSectionFactory, CssSectionFactory>();
            ServiceLocator.SetService<ICssResourceFactory, CssResourceFactory>();
        }
    }
}
