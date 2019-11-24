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
    public List<BrickSettings> wallBricks;

    public Wall(int numberOfBricks)
    {

        wallBricks = new List<BrickSettings>();

        for (int i = 0; i < numberOfBricks; i++)
        {
            wallBricks.Add(new BrickSettings());
        }
    }
}

[System.Serializable]
/// <summary>
/// Brick Parameters
/// </summary>
public struct BrickSettings
{
    public int brickID;
    public Vector3 brickPosition;
    public Vector3[] waypointsStorage;
}
