using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalBallScript2 : MonoBehaviour
{
    public Transform returnPoint;

    public float bounciness;
    public float hitSpeedMultiplier;
    public float magicalBounceSpeed;

    public float initialGravity;
    public float afterRacketHitGravity;
    public float afterFrontWallBounceGravity;
    public float afterFloorBounceGravity;

    public float maxSpeed;
    public float minSpeed;

    
    public float verticalCompensationSlope;
    public float verticalCompensationOffset;
    public bool vcIsPositive;

    public float verticalBounceSlope;
    public float verticalBounceOffset;

    public float depthCompensation;
    public float depthOffset;

    public float attractionStrength;

    //public bool isResetable; // Nom à changer

    private Rigidbody rigidbody;
    //private Vector3 resetPosition;
    private BallLastInterraction ballState;


    private Vector3 lastVelocity;
    private float floorBounce;

    private Coroutine sideAttraction;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //resetPosition = gameObject.transform.position;
        ballState = BallLastInterraction.NONE;
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)

        if (ballState == BallLastInterraction.NONE)
            rigidbody.AddForce(initialGravity * Vector3.down);
        else if (ballState == BallLastInterraction.RACKET)
            rigidbody.AddForce(afterRacketHitGravity * Vector3.down);
        else if (ballState == BallLastInterraction.FRONTWALL)
            rigidbody.AddForce(afterFrontWallBounceGravity * Vector3.down);
        else
            rigidbody.AddForce(afterFloorBounceGravity * Vector3.down);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Racket"))
        {
            StartCoroutine(RacketHit());
            ballState = BallLastInterraction.RACKET;
        }
        else if (other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick"))
        {
            MagicalBounce2(other);
            ballState = BallLastInterraction.FRONTWALL;
        }
        else if (other.gameObject.CompareTag("Floor"))
        {
            if(ballState != BallLastInterraction.FRONTWALL)
            {
                //do something special?
                Bounce(other.GetContact(0));
            }
        }
        else
            Bounce(other.GetContact(0));
    }

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet
    private void Bounce(ContactPoint contactPoint)
    {
        Vector3 normal = contactPoint.normal;
        float normalVelocity = Vector3.Dot(normal, lastVelocity);

        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);

        rigidbody.velocity = bounciness * (tangentVelocity * tangent - normalVelocity * normal);
    }


    private void MagicalBounce2(Collision collision)
    {
        Vector3 hittedPoint = collision.GetContact(0).point;
        float verticalVelocity = VerticalCompensation(hittedPoint.y);

        floorBounce = FloorBounceCompensation(hittedPoint.y);

        float depthVelocity = DepthCompensation(hittedPoint.y);

        rigidbody.velocity = new Vector3(Vector3.Dot(lastVelocity, Vector3.right), verticalVelocity, depthVelocity);
        

    }

    private IEnumerator RacketHit()
    {
        Transform currentPosition = gameObject.transform;
        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnHitEvent(gameObject);
        yield return new WaitForFixedUpdate();

        Vector3 newVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        rigidbody.position = currentPosition.position + newVelocity * Time.fixedDeltaTime * hitSpeedMultiplier;
        rigidbody.velocity = ClampVelocity(newVelocity * hitSpeedMultiplier);
    }

    private float VerticalCompensation(float hitHeigth)
    {
        float compensation = verticalCompensationSlope * hitHeigth + verticalCompensationOffset;
        if (vcIsPositive && compensation < 0)
        {
            return 0;
        }
        return compensation;
    }

    private IEnumerator SweetSpotSideAttraction()
    {
        while(true)
        {
            rigidbody.AddForce(attractionStrength * (transform.position - targetPoint));
            yield return new WaitForFixedUpdate();
        }
    }

    private float FloorBounceCompensation(float hitHeigth)
    {
        float adjustedBounce = verticalBounceSlope * hitHeigth + verticalBounceOffset;

        if (adjustedBounce < 0)
        {
            return 0; // pour eviter les bugs...?
        }
        return adjustedBounce;
    }

    private float DepthCompensation(float hitHeigth)
    {
        return depthCompensation * hitHeigth + depthOffset;
    }

    private Vector3 ClampVelocity(Vector3 calculateVelocity)
    {
        if (calculateVelocity.magnitude < minSpeed)
        {
            return minSpeed * Vector3.Normalize(calculateVelocity);
        }
        else if (calculateVelocity.magnitude > maxSpeed)
        {
            return maxSpeed * Vector3.Normalize(calculateVelocity);
        }
        else
            return calculateVelocity;
    }
    
}
