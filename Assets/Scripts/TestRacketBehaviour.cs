using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRacketBehaviour : MonoBehaviour
{
    public GameObject Racket; 

    private Vector3 positionTMinus2;
    private Vector3 positionTMinus1;
    private float lastFixedDeltaT;

    private void Start()
    {
        positionTMinus1 = gameObject.transform.position;
        positionTMinus2 = gameObject.transform.position;
        lastFixedDeltaT = 1;
    }

    void FixedUpdate()
    {
        positionTMinus2 = positionTMinus1;
        positionTMinus1 = gameObject.transform.position;
        lastFixedDeltaT = Time.fixedDeltaTime;
    }

    public Vector3 GetVelocity()
    {
        return ((positionTMinus1 - positionTMinus2) / lastFixedDeltaT + (gameObject.transform.position) / Time.fixedDeltaTime)/2;
    }
}
