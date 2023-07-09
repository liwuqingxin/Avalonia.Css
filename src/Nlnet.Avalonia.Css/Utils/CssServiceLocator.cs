using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Nlnet.Avalonia.Css
{
    public static class CssServiceLocator
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

        static CssServiceLocator()
        {
            CssServiceLocator.SetService<ITypeResolverManager, TypeResolverManager>();
            CssServiceLocator.SetService<ICssParser, CssParser>();
            CssServiceLocator.SetService<ICssInterpreter, CssInterpreter>();
            CssServiceLocator.SetService<ICssConfiguration, CssConfiguration>();
            CssServiceLocator.SetService<ICssSectionFactory, CssSectionFactory>();
            CssServiceLocator.SetService<ICssResourceFactory, CssResourceFactory>();
        }
    }
}
