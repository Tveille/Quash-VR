using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBrickBehaviour : MonoBehaviour
{
    [Header("Score Modifier")]
    public int scoreValue;




    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Ball")
        {
            BrickManager.Instance.DeadBrick(this.gameObject, scoreValue);
        }
    }
}
