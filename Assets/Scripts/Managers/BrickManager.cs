using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickManager : MonoBehaviour
{
    [Header("Score")]
    public float totalScore;
    public float currentScore;

    [Header("Destructible Bricks per Wall")]
    public List<int> DestructibeBricksPerLayer;
}
