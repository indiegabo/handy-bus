using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IndieGabo.HandyTools.HandyServiceLocator
{
    [AddComponentMenu("Handy Service Locator/ServiceLocator [Global]")]
    public class ServiceLocatorGlobalBootstrapper : ServiceBootstrapper
    {
        [SerializeField] protected bool _dontDestroyOnLoad = true;

        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(_dontDestroyOnLoad);
        }
    }
}
