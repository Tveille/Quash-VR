using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level", menuName = "Custom/Level", order = 120)]
[System.Serializable]
public class LevelsScriptable : ScriptableObject
{
    public LevelSettings[] levels;
}

public class LevelSettings
{
    public WallBuilds levelWallBuilds;
    public LayerZoneConfigurations levelZoneConfig;
}
