using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{

    [Header("Score")]
    public float score;
    public TextMeshProUGUI displayScore;



    public static ScoreManager Instance;




    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Incremente le score
    /// </summary>
    /// <param name="brickValue">Brick value for the score</param>
    public void IncrementScore(int brickValue)
    {
        score += brickValue;

        string textScore = score.ToString();

        displayScore.text = textScore;
    }
}
