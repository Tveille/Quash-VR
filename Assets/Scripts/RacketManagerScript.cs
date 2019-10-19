using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketManagerScript : MonoBehaviour
{
    //public GameObject racketPrefab;
    //public Transform racketSpawn;

    public GameObject racket;

    private Vector3 positionTMinus2;
    private Vector3 positionTMinus1;
    private float lastFixedDeltaT; // Peut être ailleur?

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

    public Vector3 GetVelocity()
    {
        Vector3 velocity = ((positionTMinus1 - positionTMinus2) / lastFixedDeltaT + (gameObject.transform.position) / Time.fixedDeltaTime) / 2;
        Debug.Log(velocity);
        return velocity;
    }
}
