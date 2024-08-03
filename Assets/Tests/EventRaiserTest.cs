using UnityEngine;
using IndieGabo.HandyBus;

public class EventRaiserTest : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EventBus<PlayerEvent>.Raise(new PlayerEvent { health = 10, mana = 10 });
        }
    }
}