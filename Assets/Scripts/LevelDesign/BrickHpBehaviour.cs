using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHpBehaviour : MonoBehaviour
{
    [Header("Wall Layer")]
    public int wallLayerID = 0;

    [Header("Score Modifier")]
    public float scoreValue;

    [Header("Health")]
    public int health = 2;
}
