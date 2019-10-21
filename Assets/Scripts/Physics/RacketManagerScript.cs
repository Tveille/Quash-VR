using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketManagerScript : MonoBehaviour
{
    //public GameObject racketPrefab;
    //public Transform racketSpawn;

    public GameObject racket;
    public float deltaHitTime = 0.5f; //Valeur A twik


    private Vector3 positionTMinus2;
    private Vector3 positionTMinus1;
    private float lastFixedDeltaT; // Peut être ailleur?

    private bool isBeingGrabbed;
    private bool isGrabbed;

    private void Start()
    {
        //racket = Instantiate(racketPrefab, racketSpawn) as GameObject;

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

    public void ActionCall(/*Player*/)
    {
        StartCoroutine(PerformAction());
    }

    private IEnumerator PerformAction()
    {
        // Abonnement au release
        while (true)
        {
            if (!isBeingGrabbed && !isGrabbed)
            {
                isBeingGrabbed = true;
                StartCoroutine(racket.GetComponent<TestRacketBehaviour>().RacketCallBack());
                break;
            }

            yield return new WaitForFixedUpdate();
        }
    }



    public void StopCall()
    {
        if (isBeingGrabbed)
            StopCoroutine(racket.GetComponent<TestRacketBehaviour>().RacketCallBack());
        else
        {
            StopCoroutine(PerformAction());
        }               
    }

    public void HitEvent(GameObject hitObject)
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
