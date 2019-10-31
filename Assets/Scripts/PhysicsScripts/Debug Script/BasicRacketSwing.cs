using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicRacketSwing : MonoBehaviour
{
    public Vector3 swingVelocity;
    public float swingDuration;

    private float swingEndingTime;

    private void Start()
    {
        swingEndingTime = Time.time + swingDuration;
    }
    void FixedUpdate()
    {
        if(Time.time < swingEndingTime)
        {
            transform.position += swingVelocity * Time.fixedDeltaTime;
        }
    }
}
