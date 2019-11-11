using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A faire : Utiliser Vector3.Reflect au lieu de calculer à la main
public class MagicalBallScript2 : MonoBehaviour
{
    public float bounciness;
    public bool switchPhysic;
    [Header("Speed Settings")]
    public float maxSpeed;
    public float minSpeed;
    public float hitSpeedMultiplier;

    [Header("Gravity Settings")]
    public float initialGravity;
    public float afterRacketHitGravity;
    public float afterFrontWallBounceGravity;
    public float afterFloorBounceGravity;

    [Header("MagicBounce Vertical Settings")]
    public float verticalCompensationSlope;
    public float verticalCompensationOffset;
    public bool vcIsPositive;

    [Header("MagicBounce Depth Settings")]
    public float depthCompensationSlope;
    public float depthCompensationOffset;

    [Header("FloorBounce Settings")]
    public float verticalBounceSlope;
    public float verticalBounceOffset;

    [Header("Attraction Settings")]
    public float attractionStrength;
    public Transform targetPoint;

    //public bool isResetable; // Nom à changer

    private Rigidbody rigidbody;
    //private Vector3 resetPosition;
    private BallLastInterraction ballState;


    private Vector3 lastVelocity;

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
            if(switchPhysic)
            {
                RacketBasicPhysicHit(other);
            }
            else
            {
                RacketArcadeHit();
            }
            
            ballState = BallLastInterraction.RACKET;
        }
        else if (other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick"))
        {
            MagicalBounce2(other);
            ballState = BallLastInterraction.FRONTWALL;
        }
        else if (other.gameObject.CompareTag("Floor"))
        {
            FloorBounce(other.GetContact(0));
            ballState = BallLastInterraction.FLOOR;
        }
        else if(other.gameObject.CompareTag("Wall"))
        {
            StandardBounce(other.GetContact(0));
        }
        else
            StandardBounce(other.GetContact(0));        // Util?
    }

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet
    private void StandardBounce(ContactPoint contactPoint)
    {
        Vector3 normal = contactPoint.normal;
        float normalVelocity = Vector3.Dot(normal, lastVelocity);

        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);

        rigidbody.velocity = bounciness * (tangentVelocity * tangent - normalVelocity * normal);
    }

    private void FloorBounce(ContactPoint contactPoint) // Attention valable que pour sol plat!
    {
        Vector3 normal = contactPoint.normal;
        float normalVelocity = AdjustFloorBounce(Vector3.Dot(normal, lastVelocity));

        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);

        rigidbody.velocity = bounciness * (tangentVelocity * tangent - normalVelocity * normal);
    }

    private void MagicalBounce2(Collision collision)
    {
        Vector3 hittedPoint = collision.GetContact(0).point;
        float verticalVelocity = CalculateVerticalBounceVelocity(hittedPoint.y);
        Debug.Log(verticalVelocity);
        //floorBounce = AdjustFloorBounce(hittedPoint.y);

        float depthVelocity = - CalculateDepthBounceVelocity(hittedPoint.y);
        Debug.Log(depthVelocity);

        float sideVelocity = - collision.GetContact(0).point.x;
        rigidbody.velocity = new Vector3(Vector3.Dot(lastVelocity, Vector3.right), verticalVelocity, depthVelocity);
        Debug.Log(rigidbody.velocity);
        //sideAttraction = StartCoroutine(SideAttractionCoroutine());
    }

    private void RacketArcadeHit()
    {
        Transform currentPosition = gameObject.transform;
        GameObject.Find("RacketManager").GetComponent<RacketManager>().OnHitEvent(gameObject);
       
        Vector3 newVelocity = RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        rigidbody.position = currentPosition.position + newVelocity * Time.fixedDeltaTime * hitSpeedMultiplier;
        rigidbody.velocity = ClampVelocity(newVelocity * hitSpeedMultiplier);
    }

    private void RacketBasicPhysicHit(Collision other) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;
        Vector3 contactPointNormal = Vector3.Normalize(other.GetContact(0).normal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;

        Vector3 tangentVelocity = relativeVelocity - normalVelocity;        // Ajouter frottement

        rigidbody.velocity = hitSpeedMultiplier * (-normalVelocity + tangentVelocity);

        GameObject.Find("RacketManager").GetComponent<RacketManager>().OnHitEvent(gameObject);  // Ignore collision pour quelque frame.
    }

    private float CalculateVerticalBounceVelocity(float hitHeigth)
    {
        float compensation = MakeLinearAssociation(hitHeigth, verticalCompensationSlope, verticalCompensationOffset);
        if (vcIsPositive && compensation < 0)
        {
            return 0;
        }
        return compensation;
    }

    private IEnumerator SideAttractionCoroutine()
    {
        while(true)
        {
            rigidbody.AddForce(attractionStrength * (transform.position.x - targetPoint.position.x) * Vector3.right); 
            yield return new WaitForFixedUpdate();
        }
    }

    private float AdjustFloorBounce(float verticalVelocity)
    {
        float adjustedBounce = MakeLinearAssociation(verticalVelocity, verticalBounceSlope, verticalBounceOffset);

        if (adjustedBounce < 0)
        {
            return 0;                                                           // pour eviter les bugs...?
        }
        return adjustedBounce;
    }

    private float CalculateDepthBounceVelocity(float hitHeigth)
    {
        return MakeLinearAssociation(hitHeigth, depthCompensationSlope, depthCompensationOffset);
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

    private float MakeLinearAssociation(float variable, float slope, float offset)
    {
        return slope * variable + offset;
    }
}
