using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Every patterns for each Layer
/// </summary>
public class LayerZoneConfigurations : ScriptableObject
{
    public Configuration[] layersZoneConfiguration;
}

/// <summary>
/// Patterns of zones for One layer
/// </summary>
public class Configuration
{
    public Rounds[] configuration;
}

/// <summary>
/// Pattern of zones
/// </summary>
public class Rounds
{
    public ZoneSettings[] round;
    public float delayBeforeNextRound;
}

/// <summary>
/// Zone Parameters
/// </summary>
public struct ZoneSettings
{
    public int zoneID;
    public Vector3 zonePosition;
    public float duration;
}
