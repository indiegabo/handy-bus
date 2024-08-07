using System.Collections;
using System.Collections.Generic;
using IndieGabo.HandyTools.HandyServiceLocator;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class LevelManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.ForSceneOf(this).Register(this);
    }

    public void Broadcast()
    {
        Debug.Log($"{gameObject.name} Broadcasted");
    }
}
