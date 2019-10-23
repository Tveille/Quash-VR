using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class InitialVelocity : MonoBehaviour
{
    public Vector3 initialVelocity;
    private Rigidbody rigidbody;
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.velocity = initialVelocity;
    }
}
