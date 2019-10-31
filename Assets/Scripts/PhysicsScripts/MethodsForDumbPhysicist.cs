using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MethodsForDumbPhysicist : MonoBehaviour
{
    public float gravity;
    public float bounciness;

    private float CalculateLandingTime(Vector3 startPoint, Vector3 velocity)
    {
        float bounceTime;

        bounceTime = ( velocity.y + Mathf.Sqrt(velocity.y + 2 * gravity * (startPoint.y - gameObject.transform.lossyScale.y / 2)) ) / gravity; // Verifier le offset 

        return bounceTime;
    }

    private Vector3 CalculateLandCoordinate(Vector3 startPoint, Vector3 velocity, float landingTime)
    {
        float x = velocity.x * landingTime + startPoint.x;
        float z = velocity.y * landingTime + startPoint.z;

        return new Vector3(x, 0f, z);
    }

}
