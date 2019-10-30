using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicalBallScriptV3 : MonoBehaviour
{   
    private enum BallState
    {
        NORMAL,
        SLOW
    }

    public enum RacketInteractionType
    {
        BASICARCADE,
        BASICPHYSIC,
        MEDIUMPHYSIC
    }
    
    
    

    [Header("Racket Settings")]
    public RacketInteractionType physicsUsed;
    public float hitMaxSpeed;
    public float hitMinSpeed;
    public float hitSpeedMultiplier;
    public float frottementRacket;

    [Header("Slow Return Settings")]
    public float slowness;              //Modider le nom?

    [Header("Gravity Settings")]
    public float gravity;
    //public float afterRacketHitGravity;
    //public float afterFrontWallBounceGravity;
    //public float afterFloorBounceGravity;

    //[Header("MagicBounce Vertical Settings")]
    //public float verticalCompensationSlope;
    //public float verticalCompensationOffset;
    //public bool vcIsPositive;

    [Header("MagicBounce Depth Settings")]
    public float depthVelocity;
    public Transform zFloorBounceTarget;

    [Header("X Return Settings")]
    public Transform xReturnTarget;

    [Header("Bounce Settings")]
    public float bounciness;
    public float frottementDynamique;



    //public bool isResetable; // Nom à changer
    //private Vector3 resetPosition;

    private Rigidbody rigidbody;
    
    private BallState ballState;

    private Vector3 lastVelocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //resetPosition = gameObject.transform.position;
        ballState = BallState.NORMAL;
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)

        if (ballState == BallState.NORMAL)
            rigidbody.AddForce(gravity * Vector3.down);
        else if (ballState == BallState.SLOW)                                   // A changer
            rigidbody.AddForce(gravity / (slowness * slowness) * Vector3.down);
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Racket"))
        {
            switch (physicsUsed)
            {
                case RacketInteractionType.BASICARCADE:
                    RacketHit(other);
                    break;
                
                case RacketInteractionType.BASICPHYSIC:
                    StartCoroutine(RacketHitCoroutine());
                    break;
                    
                case RacketInteractionType.MEDIUMPHYSIC:
                    RacketHitMediumPhysics(other);
                    break;
            }
            

            ballState = BallState.NORMAL;
        }
        else if (other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick"))
        {
            MagicalBounce3(other);
            ballState = BallState.SLOW;
        }
        else
            StandardBounce(other.GetContact(0));        // Util?
    }

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet
    private void StandardBounce(ContactPoint contactPoint)
    {
        Vector3 normal = Vector3.Normalize(contactPoint.normal);
        float normalVelocity = Vector3.Dot(normal, lastVelocity);
        Debug.Log(normalVelocity * normal);
        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);
        Debug.Log(tangentVelocity * tangent);

        rigidbody.velocity = ((1 - frottementDynamique) * tangentVelocity * tangent - bounciness * normalVelocity * normal);
        Debug.Log(rigidbody.velocity);
    }

    private void MagicalBounce3(Collision collision)
    {
        float verticalVelocity = CalculateVerticalBounceVelocity(collision);
        //Debug.Log(verticalVelocity);

        float sideVelocity = CalculateSideBounceVelocity(collision);
        //Debug.Log(sideVelocity);

        rigidbody.velocity = new Vector3(sideVelocity, verticalVelocity, -depthVelocity) / slowness;
        //Debug.Log(rigidbody.velocity);
    }

    private IEnumerator RacketHitCoroutine()
    {
        Transform currentPosition = gameObject.transform;
        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnHitEvent(gameObject);
        yield return new WaitForFixedUpdate();

        Vector3 newVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        newVelocity = ClampVelocity(newVelocity * hitSpeedMultiplier);
        rigidbody.position = currentPosition.position + newVelocity  * Time.fixedDeltaTime;
        rigidbody.velocity = newVelocity;
    }

    private void RacketHit(Collision other) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        //Transform currentPosition = gameObject.transform;
        Vector3 racketVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;


        //Debug.Log(other.contactCount);
        //Debug.Log(other.GetContact(0).normal);
        Vector3 contactPointNormal = Vector3.Normalize(other.GetContact(0).normal);
        //Debug.Log(contactPointNormal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;

        Vector3 tangentVelocity = relativeVelocity - normalVelocity;        // Ajouter frottement

        rigidbody.velocity = ClampVelocity(hitSpeedMultiplier * (-normalVelocity + tangentVelocity));

        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnHitEvent(gameObject);  // Ignore collision pour quelque frame.
    }

    private void RacketHitMediumPhysics(Collision other) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;


        //Debug.Log(other.contactCount);
        //Debug.Log(other.GetContact(0).normal);
        Vector3 contactPointNormal = Vector3.Normalize(other.GetContact(0).normal);
        //Debug.Log(contactPointNormal);

        Vector3 normalVelocity = (2 * Vector3.Dot(contactPointNormal, racketVelocity) - Vector3.Dot(contactPointNormal, lastVelocity)) * hitSpeedMultiplier * contactPointNormal;

        Vector3 tangentVelocity = (lastVelocity - Vector3.Dot(contactPointNormal, lastVelocity) * contactPointNormal) * (1 - frottementRacket);        // Ajouter frottement

        rigidbody.velocity = ClampVelocity(hitSpeedMultiplier * (-normalVelocity + tangentVelocity));

        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnHitEvent(gameObject);  // Ignore collision pour quelque frame.
    }

    private float CalculateVerticalBounceVelocity(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;
        return (gravity * (zFloorBounceTarget.position.z - transform.position.z) / -depthVelocity / 2) - (transform.position.y * -depthVelocity / (zFloorBounceTarget.position.z - transform.position.z));
    }

    private float CalculateSideBounceVelocity(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;
        Vector3 returnHorizontalDirection = new Vector3(xReturnTarget.position.x - collisionPoint.x, 0, xReturnTarget.position.z - collisionPoint.z);
        returnHorizontalDirection = Vector3.Normalize(returnHorizontalDirection);
        return Vector3.Dot(depthVelocity * Vector3.back, returnHorizontalDirection) * Vector3.Dot(returnHorizontalDirection, Vector3.right);
    }

    private Vector3 ClampVelocity(Vector3 calculateVelocity)        //Nom à modifier
    {
        if (calculateVelocity.magnitude < hitMinSpeed)
        {
            return hitMinSpeed * Vector3.Normalize(calculateVelocity);
        }
        else if (calculateVelocity.magnitude > hitMaxSpeed)
        {
            return hitMaxSpeed * Vector3.Normalize(calculateVelocity);
        }
        else
            return calculateVelocity;
    }

    private float MakeLinearAssociation(float variable, float slope, float offset)
    {
        return slope * variable + offset;
    }
}
