using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventListener : MonoBehaviour
{
    [SerializeField] private GameEvent gameEvent;
    [SerializeField] private UnityEvent<Transform, Transform, float> response;

    public void OnEventOccured(Transform playerFrom, Transform playerTo, float duration)
    {
        response.Invoke(playerFrom, playerTo, duration);
    }
    public void OnEventOccured(Transform playerPosOnExit)
    {
        response.Invoke(playerPosOnExit, playerPosOnExit, Time.deltaTime);
    }
    private void OnEnable()
    {
        gameEvent.Register(this);
    }
    private void OnDisable()
    {
        gameEvent.Unregister(this);
    }
}
