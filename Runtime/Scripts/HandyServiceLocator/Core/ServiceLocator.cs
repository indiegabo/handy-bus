using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace IndieGabo.HandyTools.HandyServiceLocator
{
    public class ServiceLocator : MonoBehaviour
    {
        const string k_GlobalServiceLocatorName = "ServiceLocator [Global]";
        const string k_SceneServiceLocatorName = "ServiceLocator [Scene]";

        static ServiceLocator _global;
        static Dictionary<Scene, ServiceLocator> _scenesContainer = new();
        static List<GameObject> _tmpSceneGameObjects = new();

        readonly ServiceManager _services = new();

        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (_global == this)
            {
                Debug.LogWarning("<color=#FFFFFF>[Handy Service Locator]</color> ServiceLocator already configured as global");
            }
            else if (_global != null)
            {
                Debug.LogError("<color=#FFFFFF>[Handy Service Locator]</color> Another ServiceLocator already configured as global");
            }
            else
            {
                _global = this;
                if (dontDestroyOnLoad)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
        }

        internal void ConfigureAsScene()
        {
            Scene scene = gameObject.scene;

            if (_scenesContainer.ContainsKey(scene))
            {
                Debug.LogError("<color=#FFFFFF>[Handy Service Locator]</color> ServiceLocator already configured as scene");
                return;
            }

            _scenesContainer.Add(scene, this);
        }

        public static ServiceLocator Global
        {
            get
            {
                if (_global != null) return _global;

                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return _global;
                }

                var container = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return _global;
            }
        }

        public static ServiceLocator For(MonoBehaviour mb)
        {
            var componentInParent = mb.GetComponentInParent<ServiceLocator>();
            if (componentInParent != null)
            {
                return componentInParent;
            }

            var inTheScene = ForSceneOf(mb);
            if (inTheScene != null)
            {
                return inTheScene;
            }

            return _global;
        }

        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            Scene scene = mb.gameObject.scene;

            if (_scenesContainer.TryGetValue(scene, out ServiceLocator container) && container != mb)
            {
                return container;
            }

            _tmpSceneGameObjects.Clear();

            scene.GetRootGameObjects(_tmpSceneGameObjects);

            foreach (GameObject go in _tmpSceneGameObjects.Where(go => go.GetComponent<ServiceLocator>() != null))
            {
                if (go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return _global;
        }

        public ServiceLocator Register<T>(T service) where T : class
        {
            _services.Register(service);
            return this;
        }

        public ServiceLocator Register(Type type, object service)
        {
            _services.Register(type, service);
            return this;
        }

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered");
        }

        private bool TryGetService<T>(out T service) where T : class
        {
            return _services.TryGet(out service);
        }

        private bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == _global)
            {
                container = null;
                return false;
            }

            if (transform.parent != null)
            {
                container = transform.parent.GetComponentInParent<ServiceLocator>();
                return container != null;
            }
            else
            {
                container = ForSceneOf(this);
                return container != null;
            }
        }

        private void OnDestroy()
        {
            if (_global == this)
            {
                _global = null;
            }
            else if (_scenesContainer.ContainsValue(this))
            {
                _scenesContainer.Remove(gameObject.scene);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            _global = null;
            _scenesContainer.Clear();
            _tmpSceneGameObjects.Clear();
        }

#if UNITY_EDITOR
        [MenuItem("GameObject/Handy Service Locator/Add Global", false, 100)]
        public static void AddGlobalServiceLocator()
        {
            var go = new GameObject(k_GlobalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [MenuItem("GameObject/Handy Service Locator/Add Scene", false, 100)]
        public static void AddSceneServiceLocator()
        {
            var go = new GameObject(k_SceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif
    }
}
