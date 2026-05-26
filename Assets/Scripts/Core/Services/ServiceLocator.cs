using System;
using System.Collections.Generic;

namespace Core.Services
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, IService> Services = new();

        public static void Register<TService>(TService service) where TService : class, IService
        {
            Type type = typeof(TService);

            if (Services.ContainsKey(type))
                Services[type] = service;
            else
                Services.Add(type, service);
        }

        public static TService Get<TService>() where TService : class, IService
        {
            Type type = typeof(TService);

            if (!Services.TryGetValue(type, out IService service))
                throw new InvalidOperationException($"Service {type.Name} is not registered.");

            return service as TService;
        }

        public static bool TryGet<TService>(out TService service) where TService : class, IService
        {
            if (Services.TryGetValue(typeof(TService), out IService rawService))
            {
                service = rawService as TService;
                return service != null;
            }

            service = null;
            return false;
        }

        public static void Clear()
        {
            Services.Clear();
        }
    }
}