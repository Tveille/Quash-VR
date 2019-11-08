using System.Collections;
using System.Collections.Generic;
using UnityEngine;




/// <summary>
/// Every walls for One level
/// </summary>
public class WallBuilds : ScriptableObject
{
    public Wall[] walls;
}

/// <summary>
/// One Wall Composition of bricks
/// </summary>
public class Wall
{
    public BrickSettings[] wallBricks;
}


/// <summary>
/// Brick Parameters
/// </summary>
public struct BrickSettings
{
    int brickID;
    Vector3 brickPosition;
    Vector3[] waypointsStorage;
}
