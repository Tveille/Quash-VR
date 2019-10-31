using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerZoneConfigurations : ScriptableObject
{
    public ZoneSettings[] configurations;
    public float delayBeforeNextConfiguration;
}

public struct ZoneSettings
{
    public int zoneID;
    public Vector3 zonePosition;
    public float duration;
}
