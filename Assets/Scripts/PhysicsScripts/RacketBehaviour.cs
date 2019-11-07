using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketBehaviour : MonoBehaviour
{
    public Transform grabDefaultTransform;
    public float maxGrabDistance;
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
            Vector3 destinationVector = QPlayerManager.instance.GetRightController(PlayerID.PLAYER1).transform.position - gameObject.transform.position;

            if (destinationVector.magnitude <= maxGrabDistance)
            {
                BecomeGrabbed();
                break;
            }

            float remainingTime = returnDuration - (Time.time - returnStartingTime);
            
            if (remainingTime <= 0)
                gameObject.transform.position = QPlayerManager.instance.GetRightController(PlayerID.PLAYER1).transform.position;
            else
                gameObject.transform.position += destinationVector * Time.fixedDeltaTime / remainingTime;
                
            yield return new WaitForFixedUpdate();
        }
    }

    public void BecomeGrabbed()
    {
        RacketManager.instance.OnRacketGrab();
        transform.parent = QPlayerManager.instance.GetRightController(PlayerID.PLAYER1).transform;
        
        if (grabDefaultTransform == null)
        {
            transform.localPosition = new Vector3(0,0,-0.2f);
            transform.localEulerAngles = new Vector3(180,0,90);
        }
        else
        {
            transform.localPosition = grabDefaultTransform.position;
            transform.rotation = grabDefaultTransform.rotation;
        }
    }

    public void BecomeUngrabbed()
    {
        transform.parent = null;
    }
}
