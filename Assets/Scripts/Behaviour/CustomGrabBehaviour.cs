using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct GrabInfo
{
    public PlayerID userID;
    public PlayerHand userHand;
    public GrabState grabState;
}

public class CustomGrabBehaviour : MonoBehaviour
{
    public Transform grabDefaultTransform;
    public float maxGrabDistance;
    public float returnDuration;
    
    private Rigidbody rigidbody;
    private bool hasRigidbody;
    private float returnStartingTime;


    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
        if (rigidbody == null)
            hasRigidbody = false;
        else
            hasRigidbody = true;
    }

    public IEnumerator Attraction(IGrabCaller grabCaller)
    {
        GrabInfo grabInfo = grabCaller.GetGrabInfo();
        returnStartingTime = Time.time;

        while (grabInfo.grabState != GrabState.GRABBED)     
        {
            Vector3 destinationVector = QPlayerManager.instance.GetController(grabInfo.userID, grabInfo.userHand).transform.position - gameObject.transform.position;

            if (destinationVector.magnitude <= maxGrabDistance)
            {
                BecomeGrabbed(grabCaller);
                break;
            }

            float remainingTime = returnDuration - (Time.time - returnStartingTime);

            if (remainingTime <= 0)
                gameObject.transform.position = QPlayerManager.instance.GetController(grabInfo.userID, grabInfo.userHand).transform.position;
            else
                gameObject.transform.position += destinationVector * Time.fixedDeltaTime / remainingTime;

            yield return new WaitForFixedUpdate();
        }
    }

    public void BecomeGrabbed(IGrabCaller grabCaller)
    {
        grabCaller.OnGrab();

        GrabInfo grabInfo = grabCaller.GetGrabInfo();
        transform.parent = QPlayerManager.instance.GetController(grabInfo.userID, grabInfo.userHand).transform;

        if (grabDefaultTransform == null)
        {
            transform.localPosition = Vector3.zero;
            transform.localEulerAngles = Vector3.zero;
        }
        else
        {
            transform.localPosition = grabDefaultTransform.localPosition;
            transform.rotation = grabDefaultTransform.localRotation;
        }
    }

    public void BecomeUngrabbed(IGrabCaller grabCaller)
    {
        transform.parent = null;
        grabCaller.OnUngrab();
    }
}
