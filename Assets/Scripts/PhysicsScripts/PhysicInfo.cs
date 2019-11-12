using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicInfo : MonoBehaviour
{
    private Vector3 positionTMinus1;
    private Quaternion rotationTMinus1;

    private Vector3 velocityTMinus3Half;
    private Vector3 velocityTMinusHalf;
    private Vector3 angularVelocityTMinus3half;
    private Vector3 angularVelocityTMinusHalf;

    private Vector3 accelerationTMinus1;
    private Vector3 angularAccelerationTMinus1;

    private float dTMinus1;

    private void Start()
    {
        positionTMinus1 = transform.position;

        velocityTMinus3Half = Vector3.zero;
        velocityTMinusHalf = Vector3.zero;
        angularVelocityTMinus3half = Vector3.zero;
        angularVelocityTMinusHalf = Vector3.zero;

        accelerationTMinus1 = Vector3.zero;
        angularAccelerationTMinus1 = Vector3.zero;

        rotationTMinus1 = transform.rotation;

        dTMinus1 = 1;
    }

    void FixedUpdate()
    {
        velocityTMinusHalf = CalculateVelocity(transform.position, positionTMinus1, Time.fixedDeltaTime);
        angularVelocityTMinusHalf = CalculateAngularVelocity(transform.rotation, rotationTMinus1, Time.fixedDeltaTime);

        accelerationTMinus1 = CalculateAcceleration(velocityTMinusHalf, velocityTMinus3Half, Time.fixedDeltaTime, dTMinus1);
        angularAccelerationTMinus1 = CalculateAcceleration(angularVelocityTMinusHalf, angularVelocityTMinus3half, Time.fixedDeltaTime, dTMinus1);

        positionTMinus1 = transform.position;
        rotationTMinus1 = transform.rotation;

        velocityTMinus3Half = velocityTMinusHalf;
        angularVelocityTMinus3half = angularVelocityTMinusHalf;

        dTMinus1 = Time.fixedDeltaTime;
    }

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

    private Vector3 GetPosition()
    {
        return transform.position;
    }

    public Quaternion GetRotation()             // Prendre en compte la rotation de base de la raquette dans la main du joueur
    {
        return transform.rotation;
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
}
