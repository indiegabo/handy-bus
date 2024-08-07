using UnityEngine;
using IndieGabo.HandyTools.HandyBus;

public struct PlayerEvent : IEvent
{
    public int mana;
    public int health;
}

public class Hero : MonoBehaviour
{
    public int health;
    public int mana;

    private EventBinding<PlayerEvent> _playerEvent;

    private void Awake()
    {
        _playerEvent = new EventBinding<PlayerEvent>(HandlePlayerEvent);
    }

    private void OnEnable()
    {
        EventBus<PlayerEvent>.Register(_playerEvent);
    }

    private void OnDisable()
    {
        EventBus<PlayerEvent>.Deregister(_playerEvent);
    }

    void HandlePlayerEvent(PlayerEvent @event)
    {
        health = @event.health;
        mana = @event.mana;

        Debug.Log($"Health: {health}, Mana: {mana}");
    }
}