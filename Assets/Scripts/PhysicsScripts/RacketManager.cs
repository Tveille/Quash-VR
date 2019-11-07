using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class RacketManager : MonoBehaviour
{
    #region Singleton
    public static RacketManager instance;

    private void Awake()
    {
        if (instance)
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

    //public GameObject racketPrefab;
    //public Transform racketSpawn;

    public GameObject racket;
    public float deltaHitTime = 0.5f; //Valeur A twik

    private bool isBeingGrabbed;
    private bool isGrabbed;
    private PlayerID userID;

    private Vector3 positionTMinus1;
    private Quaternion rotationTMinus1;

    private Vector3 velocityTMinus3Half;
    private Vector3 velocityTMinusHalf;
    private Vector3 angularVelocityTMinus3half;
    private Vector3 angularVelocityTMinusHalf;

    private Vector3 accelerationTMinus1;
    private Vector3 angularAccelerationTMinus1;

    private float dTMinus1;
    private float dTMinus2;

    private Coroutine[] grabCallCoroutine = new Coroutine[2];
    private Coroutine racketAttractionCoroutine;

    private void Start()
    {
        //racket = Instantiate(racketPrefab, racketSpawn) as GameObject;

        isBeingGrabbed = false;
        isGrabbed = false;
        userID = PlayerID.NONE;

        positionTMinus1 = racket.transform.position;

        velocityTMinus3Half = Vector3.zero;
        velocityTMinusHalf = Vector3.zero;
        angularVelocityTMinus3half = Vector3.zero;
        angularVelocityTMinusHalf = Vector3.zero;

        accelerationTMinus1 = Vector3.zero;
        angularAccelerationTMinus1 = Vector3.zero;

        rotationTMinus1 = racket.transform.rotation;

        dTMinus1 = 1;

        //StartCoroutine(SetupEventSuscription());
    }

    void FixedUpdate()
    {
        velocityTMinusHalf = CalculateVelocity(racket.transform.position, positionTMinus1, Time.fixedDeltaTime);
        angularVelocityTMinusHalf = CalculateAngularVelocity(racket.transform.rotation, rotationTMinus1, Time.fixedDeltaTime);

        accelerationTMinus1 = CalculateAcceleration(velocityTMinusHalf, velocityTMinus3Half, Time.fixedDeltaTime, dTMinus1);
        angularAccelerationTMinus1 = CalculateAcceleration(angularVelocityTMinusHalf, angularVelocityTMinus3half, Time.fixedDeltaTime, dTMinus1);

        positionTMinus1 = racket.transform.position;
        rotationTMinus1 = racket.transform.rotation;

        velocityTMinus3Half = velocityTMinusHalf;
        angularVelocityTMinus3half = angularVelocityTMinusHalf;

        dTMinus1 = Time.fixedDeltaTime;
    }

    ///////////////////////////////////////////////////     Getter     //////////////////////////////////////////////////
    
    private Vector3 GetPosition()
    {
        return racket.transform.position;
    }

    public Quaternion GetRotation()             // Prendre en compte la rotation de base de la raquette dans la main du joueur
    {
        return racket.transform.rotation;
    }

    public Vector3 GetVelocity()
    {
        return velocityTMinusHalf;
    }

    public Vector3 GetAngularVelocity()         // Prendre en compte la rotation de base de la raquette dans la main du joueur
    {
        return angularVelocityTMinusHalf;
    }

    public Vector3 GetAcceleration()
    {
        return accelerationTMinus1;
    }

    public Vector3 GetAngularAcceleration()
    {
        return angularAccelerationTMinus1;
    }

    //////////////////////////////////////////////    Calculus Methods     /////////////////////////////////////////////

    private Vector3 CalculateVelocity(Vector3 currentPosition, Vector3 previousPosition, float deltaTime)
    {
        return (currentPosition - previousPosition) / deltaTime;
    }

    private Vector3 CalculateAngularVelocity(Quaternion currentRotation, Quaternion lastRotation, float deltaTime)      // Trouver la bonne formule...
    {
        return Vector3.zero;
    }

    private Vector3 CalculateAcceleration(Vector3 velocity2, Vector3 velocity1, float deltaTime2, float deltaTime1)
    {
        return (velocity2 - velocity1) / ((deltaTime2 + deltaTime1) / 2);
    }

    //////////////////////////////////////////////     Other Methods     //////////////////////////////////////////////

    public void OnHitEvent(GameObject hitObject)                        // Faire Un vrai event?
    {
        StartCoroutine(AfterHitIgnoreCoroutine(hitObject, Time.time));
    }

    private IEnumerator AfterHitIgnoreCoroutine(GameObject hitObject, float lastHitTime)
    {
        Physics.IgnoreCollision(racket.GetComponent<Collider>(), hitObject.GetComponent<Collider>(), true);
        while (Time.time < lastHitTime + deltaHitTime)
        {
            yield return new WaitForFixedUpdate(); // Remplacer par WaitForSeconds
        }
        Physics.IgnoreCollision(racket.GetComponent<Collider>(), hitObject.GetComponent<Collider>(), false);
    }

    //////////////////////////////////////////////     Distant Grab     //////////////////////////////////////////////
    
    public void RacketGrabCall(PlayerID callingPlayerID)
    {
        grabCallCoroutine[(int)callingPlayerID] = StartCoroutine(AttemptAttraction(callingPlayerID));
    }

    private IEnumerator AttemptAttraction(PlayerID callingPlayerID)
    {
        while (true)
        {
            if (!isBeingGrabbed && !isGrabbed)
            {
                isBeingGrabbed = true;

                racket.GetComponent<Rigidbody>().useGravity = false;
                racket.GetComponent<Rigidbody>().isKinematic = true;

                userID = callingPlayerID;
                racketAttractionCoroutine = StartCoroutine(racket.GetComponent<RacketBehaviour>().RacketCallBack(userID));
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }


    public void StopRacketGrabCall(PlayerID callingPlayerID)
    {
        if (userID == callingPlayerID)
        {
            if (isBeingGrabbed)
            {
                isBeingGrabbed = false;
                StopCoroutine(racketAttractionCoroutine);
                racket.GetComponent<Rigidbody>().useGravity = true;
                racket.GetComponent<Rigidbody>().isKinematic = false;
                racket.GetComponent<Rigidbody>().velocity = GetVelocity();
                racket.GetComponent<Rigidbody>().angularVelocity = GetAngularVelocity();
            }
            if (isGrabbed)
            {
                racket.GetComponent<RacketBehaviour>().BecomeUngrabbed();

                racket.GetComponent<Rigidbody>().useGravity = true;
                racket.GetComponent<Rigidbody>().isKinematic = false;
                racket.GetComponent<Rigidbody>().velocity = GetVelocity();
                racket.GetComponent<Rigidbody>().angularVelocity = GetAngularVelocity();

                isGrabbed = false;
            }
        }
        else
        {
            StopCoroutine(grabCallCoroutine[(int)callingPlayerID]);                                                  //Probleme stopper la bonne Coroutine ( mettre les coroutine dans des variables.
        }
        Debug.Log(GetAngularVelocity());
    }

    public bool GetGrabStatus()
    {
        return isGrabbed;
    }

    public void OnRacketGrab()
    {
        isGrabbed = true;
        isBeingGrabbed = false;
    }

    //public void OnVRTKGrab(object sender, ObjectInteractEventArgs e)
    //{
    //    StopCoroutine(racketAttractionCoroutine);
    //    isGrabbed = true;
    //}

    //public void OnGrabRelease(object sender, ObjectInteractEventArgs e)
    //{
    //    racket.GetComponent<Rigidbody>().useGravity = true;
    //    racket.GetComponent<Rigidbody>().isKinematic = false;
    //    racket.GetComponent<Rigidbody>().velocity = GetVelocity();
    //    racket.GetComponent<Rigidbody>().angularVelocity = GetAngularVelocity();

    //    isGrabbed = false;
    //}

    // C'est de la merde...
    //public IEnumerator SetupEventSuscription()
    //{
    //    yield return new WaitForFixedUpdate();
    //    yield return new WaitForFixedUpdate();
    //    Debug.Log(QPlayerManager.instance.GetRightController(PlayerID.PLAYER1).ToString());
    //    QPlayerManager.instance.GetRightController(PlayerID.PLAYER1).GetComponent<VRTK_InteractGrab>().ControllerGrabInteractableObject += OnVRTKGrab;
    //    QPlayerManager.instance.GetRightController(PlayerID.PLAYER1).GetComponent<VRTK_InteractGrab>().ControllerUngrabInteractableObject += OnVRTKGrabRelease;
    //}
}



