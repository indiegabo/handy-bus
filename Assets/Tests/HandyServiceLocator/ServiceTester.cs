using IndieGabo.HandyTools.HandyServiceLocator;
using UnityEngine;

public class ServiceTester : MonoBehaviour
{
    private LevelManager _levelManager;
    private SaveManager _saveManager;

    private void OnEnable()
    {
        ServiceLocator.For(this).Get(out _levelManager);
        ServiceLocator.Global.Get(out _saveManager);


        _levelManager.Broadcast();
        _saveManager.Save();
    }
}