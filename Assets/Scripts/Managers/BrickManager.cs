using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public WallBuilds levelWallsConfig;

    [Header("Number of bricks on the current layer")]
    public int totalBricskOnLayer;
    public int currentBricksOnLayer;

    [Header("Number of bricks in the level")]
    public int totalBricskInLevel;


    public static BrickManager Instance;




    private void Awake()
    {
        Instance = this;
    }


    /// <summary>
    /// Détruit la brique
    /// </summary>
    /// <param name="brickToDestroy">Brick that will be detroyed</param>
    /// <param name="brickValue">Brick value for the score</param>
    public void DeadBrick(GameObject brickToDestroy, int brickValue)
    {
        Vector3 brickPos = brickToDestroy.transform.position;

        brickToDestroy.SetActive(false);

        PoolManager.instance.SpawnFromPool("CubeDeathFX", brickPos, Quaternion.identity);

        ScoreManager.Instance.IncrementScore(brickValue);
    }

    void UpdateBrickLevel()
    {
        currentBricksOnLayer -= 1;

        if(currentBricksOnLayer <= 0)
        {
            LevelManager.Instance.SetNextLayer();
        }
    }

}
