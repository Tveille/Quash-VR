using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickHpBehaviour : MonoBehaviour
{
    [Header("Wall Layer")]
    public int wallLayerID = 0;

    [Header("Score Modifier")]
    public int scoreValue;

    [Header("Armor")]
    [Tooltip("How many hit BEFORE the next one kill it")]
    public int ArmorPoints = 1;



    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            if(ArmorPoints == 0)
            {
                BrickManager.Instance.DeadBrick(this.gameObject, scoreValue);
            }

            ArmorPoints--;
        }
    }
}
