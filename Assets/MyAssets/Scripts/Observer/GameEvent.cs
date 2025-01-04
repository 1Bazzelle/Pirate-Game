using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "GameEvent")]
public class GameEvent : ScriptableObject
{
    private List<EventListener> eventListeners = new();

    public void Register(EventListener listener)
    {
        eventListeners.Add(listener);
    }
    public void Unregister(EventListener listener)
    {
        eventListeners.Remove(listener);
    }

    // Make player move down a certain path
    public void Occured(Transform playerFrom, Transform playerTo, float duration)
    {
        foreach (EventListener listener in eventListeners)
        {
            listener.OnEventOccured(playerFrom, playerTo, duration);
        }
    }
    // Overload for just playing a cutscene
    public void Occured(Transform playerPosOnExit)
    {
        foreach (EventListener listener in eventListeners)
        {
            listener.OnEventOccured(playerPosOnExit);
        }
    }
}
