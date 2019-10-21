using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketManagerScript : MonoBehaviour
{
    //public GameObject racketPrefab;
    //public Transform racketSpawn;

    public GameObject racket;
    public float deltaHitTime = 0.5f; //Valeur A twik

    private bool isBeingGrabbed;
    private bool isGrabbed;
    private PlayerID userID;

    private Vector3 positionTMinus2;
    private Vector3 positionTMinus1;
    private float lastFixedDeltaT; // Peut être ailleur?


    private void Start()
    {
        //racket = Instantiate(racketPrefab, racketSpawn) as GameObject;

        isBeingGrabbed = false;
        isGrabbed = false;
        userID = PlayerID.NONE;

        positionTMinus1 = racket.transform.position;
        positionTMinus2 = racket.transform.position;
        lastFixedDeltaT = 1;
    }

    void FixedUpdate()
    {
        positionTMinus2 = positionTMinus1;
        positionTMinus1 = racket.transform.position;
        lastFixedDeltaT = Time.fixedDeltaTime;
    }

    public void OnActionCall(PlayerID callingPlayerID)
    {
        StartCoroutine(PerformAction(callingPlayerID));
    }

    private IEnumerator PerformAction(PlayerID callingPlayerID)
    {
        // Abonnement au release
        while (true)
        {
            if (!isBeingGrabbed && !isGrabbed)
            {
                isBeingGrabbed = true;
                userID = callingPlayerID;  
                StartCoroutine(racket.GetComponent<TestRacketBehaviour>().RacketCallBack(userID));
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }



    public void OnStopCall(PlayerID callingPlayerID)
    {
        if (userID == callingPlayerID)
        {
            if(isBeingGrabbed)
            {
                StopCoroutine(racket.GetComponent<TestRacketBehaviour>().RacketCallBack(callingPlayerID));
            }
            if(isGrabbed)
            {
                // Ajouter le cas du grab
            }
        }
            
        else
        {
            StopCoroutine(PerformAction(callingPlayerID));                                                  //Probleme stopper la bonne Coroutine ( mettre les coroutine dans des variables.
        }               
    }

    public void OnHitEvent(GameObject hitObject)                        // Faire Un vrai event?
    {
        StartCoroutine(BallIgnoreCoroutine(hitObject, Time.time));
    }

    private IEnumerator BallIgnoreCoroutine(GameObject hitObject, float lastHitTime)
    {
        Physics.IgnoreCollision(racket.GetComponent<Collider>(), hitObject.GetComponent<Collider>(), true);
        while(Time.time < lastHitTime + deltaHitTime)
        {
            yield return new WaitForFixedUpdate();
        }
        Physics.IgnoreCollision(racket.GetComponent<Collider>(), hitObject.GetComponent<Collider>(), false);
    }

    public Vector3 GetVelocity()
    {
        Vector3 velocity = ((positionTMinus1 - positionTMinus2) / lastFixedDeltaT + (gameObject.transform.position) / Time.fixedDeltaTime) / 2;
        Debug.Log(velocity);
        return velocity;
    }
}
