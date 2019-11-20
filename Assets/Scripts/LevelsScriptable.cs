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
