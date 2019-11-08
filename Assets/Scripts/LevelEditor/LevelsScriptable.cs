using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level Settings Asset", menuName = "Custom/LevelConfig", order = 120)]
[System.Serializable]
public class LevelsScriptable : ScriptableObject
{

    public Sprite background;
    
}

public class LevelSettings
{
    public WallBuilds levelWallBuilds;
    public LayerZoneConfigurations levelZoneConfig;
}
