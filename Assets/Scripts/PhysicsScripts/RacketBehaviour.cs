using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketBehaviour : MonoBehaviour
{
    public float returnDuration;

    private Rigidbody rigidbody;
    private float returnStartingTime;

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public IEnumerator RacketCallBack(PlayerID userID)
    {
        returnStartingTime = Time.time;

        while (!RacketManager.instance.GetGrabStatus())     // Condition à modifier
        {
            float remainingTime = returnDuration - (Time.time - returnStartingTime);
            
            if (remainingTime <= 0)
                gameObject.transform.position = PlayerManager.instance.GetRigthControllerTransform(PlayerID.PLAYER1).position;
            else
            {
                Vector3 destinationVector = PlayerManager.instance.GetRigthControllerTransform(PlayerID.PLAYER1).position - gameObject.transform.position;
                gameObject.transform.position += destinationVector * Time.fixedDeltaTime / remainingTime;
            }
                
            yield return new WaitForFixedUpdate();
        }
    }
}
