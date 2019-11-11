using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour, IGrabCaller
{
    #region Singleton
    public static BallManager instance;

    private void Awake()
    {
        if(instance)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
    #endregion

    public GameObject ball;
    public bool isResetable;
    public float resetDelay;

    private GrabInfo ballGrabInfo;
    private Coroutine delayResetCoroutine;
    private Coroutine attractionCoroutine;

    private void Start()
    {
        ballGrabInfo.userID = PlayerID.NONE;
        ballGrabInfo.grabState = GrabState.UNUSED;
    }

    public GrabInfo GetGrabInfo()
    {
        return ballGrabInfo;
    }

    public void BallResetCall(PlayerID playerID, PlayerHand playerHand)
    {
        if(ballGrabInfo.grabState == GrabState.UNUSED)
        {
            if (isResetable)
            {
                ballGrabInfo.userID = playerID;
                ballGrabInfo.userHand = playerHand;
                ballGrabInfo.grabState = GrabState.DELAYED;
                delayResetCoroutine = StartCoroutine(DelayBallReset());
            }
        }
    }

    public IEnumerator DelayBallReset()
    {
        yield return new WaitForSeconds(resetDelay);
        
        CustomGrabBehaviour grabBehaviour = ball.GetComponent<CustomGrabBehaviour>();
        if (grabBehaviour)
        {
            ballGrabInfo.grabState = GrabState.ATTRACTED;
            ball.GetComponent<Rigidbody>().useGravity = false;
            ball.GetComponent<Rigidbody>().isKinematic = true;
            attractionCoroutine = StartCoroutine(grabBehaviour.Attraction(this));
        }
            
    }

    public void BallResetStopCall(PlayerID playerID)
    {
        if (ballGrabInfo.grabState == GrabState.ATTRACTED)
        {
            if (ballGrabInfo.userID == playerID)
            {
                StopCoroutine(attractionCoroutine);
                ball.GetComponent<Rigidbody>().useGravity = true;
                ball.GetComponent<Rigidbody>().isKinematic = false;
                ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<PhysicInfo>().GetVelocity();
                ball.GetComponent<Rigidbody>().angularVelocity = ball.GetComponent<PhysicInfo>().GetAngularVelocity();
            }
        }
        else if (ballGrabInfo.grabState == GrabState.GRABBED)
        {
            if (ballGrabInfo.userID == playerID)
            {
                ball.GetComponent<CustomGrabBehaviour>().BecomeUngrabbed(this);
                ball.GetComponent<Rigidbody>().useGravity = true;
                ball.GetComponent<Rigidbody>().isKinematic = false;
                ball.GetComponent<Rigidbody>().velocity = ball.GetComponent<PhysicInfo>().GetVelocity();
                ball.GetComponent<Rigidbody>().angularVelocity = ball.GetComponent<PhysicInfo>().GetAngularVelocity();
            }
        }
        else if (ballGrabInfo.grabState == GrabState.DELAYED)
        {
            StopCoroutine(delayResetCoroutine);
        }

        ballGrabInfo.userID = PlayerID.NONE;
        ballGrabInfo.grabState = GrabState.UNUSED;
    }

    public void OnGrab()
    {
        ballGrabInfo.grabState = GrabState.GRABBED;
    }

    public void OnUngrab()
    {
        return;
    }
}
