using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon;

public class RacketBehaviour : MonoBehaviourPunCallbacks, IPunObservable
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

    public IEnumerator RacketCallBack(RacketUserInfo racketUserInfo)
    {
        returnStartingTime = Time.time;

        while (RacketManager.instance.GetGrabStatus() != GrabState.GRABBED)     // Condition à modifier
        {
            Vector3 destinationVector = QPlayerManager.instance.GetController(racketUserInfo.userID, racketUserInfo.userHand).transform.position - gameObject.transform.position;

            if (destinationVector.magnitude <= maxGrabDistance)
            {
                BecomeGrabbed(racketUserInfo);
                break;
            }

            float remainingTime = returnDuration - (Time.time - returnStartingTime);
            
            if (remainingTime <= 0)
                gameObject.transform.position = QPlayerManager.instance.GetController(racketUserInfo.userID, racketUserInfo.userHand).transform.position;
            else
                gameObject.transform.position += destinationVector * Time.fixedDeltaTime / remainingTime;
                
            yield return new WaitForFixedUpdate();
        }
    }

    public void BecomeGrabbed(RacketUserInfo racketUserInfo)
    {
        RacketManager.instance.OnRacketGrab();
        transform.parent = QPlayerManager.instance.GetController(racketUserInfo.userID, racketUserInfo.userHand).transform;
        
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

    void Update(){
        
    }

    #region  IPunObservable implementation

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
