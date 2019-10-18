using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [Header("Score")]
    public float score;

    [Header("Number of bricks on the current layer")]
    public int totalBricskOnLayer;
    public int currentBricksOnLayer;

    [Header("Number of bricks in the level")]
    public int totalBricskInLevel;

    [Header("Destructible Bricks per Wall")]
    public List<int> DestructibeBricksPerLayer;

    [Header("Death")]
    public GameObject deathParticle;




    public void DeadBrick(GameObject brickToDestroy, int brickValue)
    {

    }

    private void IncrementScore(int brickValue)
    {
        score += brickValue;
    }
}
