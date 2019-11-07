using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BrickManager : MonoBehaviour
{
    [Header("Score")]
    public float score;
    public TextMeshProUGUI displayScore;

    [Header("Number of bricks on the current layer")]
    public int totalBricskOnLayer;
    public int currentBricksOnLayer;

    [Header("Number of bricks in the level")]
    public int totalBricskInLevel;

    [Header("Destructible Bricks per Wall")]
    public List<int> DestructibeBricksPerLayer;

    //Mise en static
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

        IncrementScore(brickValue);
    }


    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    private void IncrementScore(int brickValue)
    {
        score += brickValue;

        string textScore = score.ToString();

        displayScore.text = textScore;
    }
}
