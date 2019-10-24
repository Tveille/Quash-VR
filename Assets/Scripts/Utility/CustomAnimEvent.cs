using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CustomAnimEvent : MonoBehaviour
{
    public EventAnimation[] eventAnimations;

    public void AnimEvent(int eventIndex)
    {
        eventAnimations[eventIndex].animEvent.Invoke();
    }
}

[System.Serializable]
public class EventAnimation
{
    public string eventName = "myEvent";
    public UnityEvent animEvent;

}