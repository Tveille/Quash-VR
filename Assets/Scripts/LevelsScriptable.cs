using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Malee;


/////////////////////////////  LEVEL  ////////////////////////////////

[CreateAssetMenu(fileName = "Level", menuName = "Custom/Level", order = 120)]
[System.Serializable]
public class LevelsScriptable : ScriptableObject
{
    public LevelSettings level = new LevelSettings();
}

[System.Serializable]
public class LevelSettings 
{
    public WallBuilds levelWallBuilds;
    public LayerZoneConfigurations levelZoneConfig;
}




/////////////////////////////  BRICKS  ////////////////////////////////

[System.Serializable]
/// <summary>
/// Every walls for One level
/// </summary>
public class WallBuilds
{
    public Wall[] walls;
}

[System.Serializable]
/// <summary>
/// One Wall Composition of bricks
/// </summary>
public class Wall
{
    public BrickSettings[] wallBricks;
}

[System.Serializable]
/// <summary>
/// Brick Parameters
/// </summary>
public struct BrickSettings
{
    int brickID;
    Vector3 brickPosition;
    Vector3[] waypointsStorage;
}





/////////////////////////////  ZONES  ////////////////////////////////

[System.Serializable]
/// <summary>
/// Every patterns for each Layer
/// </summary>
public class LayerZoneConfigurations
{
    public Configuration[] layersZoneConfiguration;
}

[System.Serializable]
/// <summary>
/// Patterns of zones for One layer
/// </summary>
public class Configuration
{
    public Rounds[] configuration;
}

[System.Serializable]
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
