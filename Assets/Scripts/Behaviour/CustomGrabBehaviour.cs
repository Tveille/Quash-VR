using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public struct GrabInfo
{
    public PlayerID userID;
    public PlayerHand userHand;
    public GrabState grabState;
}

public class CustomGrabBehaviour : MonoBehaviourPunCallbacks, IPunObservable
{
    public Vector3 grabDefaultPosition;
    public Vector3 grabDefaultRotation;
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

        transform.localPosition = grabDefaultPosition;
        transform.localEulerAngles = grabDefaultRotation;
    }

    public void BecomeUngrabbed(IGrabCaller grabCaller)
    {
        transform.parent = null;
        grabCaller.OnUngrab();
    }

    #region IPunObservable implementation

    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info){
        if (stream.IsWriting){
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            //stream.SendNext(transform.parent);
        }
        else{
            transform.position = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
            //transform.parent = (Transform)stream.ReceiveNext();
        }
    }

    #endregion
}
