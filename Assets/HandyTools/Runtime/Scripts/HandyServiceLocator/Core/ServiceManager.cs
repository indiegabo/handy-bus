using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieGabo.HandyTools.HandyServiceLocator
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> _services = new();
        public IEnumerable<object> RegisteredServices => _services.Values;

        public bool TryGet<T>(out T service) where T : class
        {
            Type type = typeof(T);
            if (_services.TryGetValue(type, out object obj))
            {
                service = obj as T;
                return true;
            }

            service = null;
            return false;
        }


        public T Get<T>() where T : class
        {
            Type type = typeof(T);

            if (_services.TryGetValue(type, out object obj))
            {
                return obj as T;
            }

            throw new ArgumentException($"Service of type {type.FullName} not registered");
        }

        public ServiceManager Register<T>(T service)
        {
            Type type = typeof(T);

            if (!_services.TryAdd(type, service))
            {
                Debug.LogError($"<color=#FFFFFF>[Handy Service Locator]</color> Service of type {type} already registered");
            }

            return this;
        }

        public ServiceManager Register(Type type, object service)
        {
            if (!type.IsInstanceOfType(service))
            {
                throw new ArgumentException($"Service must be of type {type}");
            }

            if (!_services.TryAdd(type, service))
            {
                Debug.LogError($"<color=#FFFFFF>[Handy Service Locator]</color> Service of type {type} already registered");
            }

            return this;
        }
    }
}
