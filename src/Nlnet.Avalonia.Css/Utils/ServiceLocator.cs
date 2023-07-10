using System;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css
{
    internal static class ServiceLocator
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
            SetService<ITypeResolverManager, TypeResolverManager>();
            SetService<ICssParser, CssParser>();
            SetService<ICssInterpreter, CssInterpreter>();
            SetService<ICssConfiguration, CssConfiguration>();
            SetService<ICssSectionFactory, CssSectionFactory>();
            SetService<ICssResourceFactory, CssResourceFactory>();
        }
    }
}
