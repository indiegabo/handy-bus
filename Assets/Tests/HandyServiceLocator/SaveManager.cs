using System.Collections;
using System.Collections.Generic;
using IndieGabo.HandyTools.HandyServiceLocator;
using UnityEngine;

[DefaultExecutionOrder(-100)]
public class SaveManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        ServiceLocator.Global.Register(this);
    }

    public void Save()
    {
        Debug.Log($"{gameObject.name} Saved");
    }
}
