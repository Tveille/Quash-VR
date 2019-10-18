using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Waypoint : MonoBehaviour
{
    [Header("Waiting Parameters")]
    public bool hasToWait;
    public float waitFor;

    [Header("Move Modifiers")]
    [Range(0,1)]
    public float smoothTime;
    [Range(0.1f, 10)]
    public float speed;
}
