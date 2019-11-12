using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BallLastInterraction
{
    NONE,
    RACKET,
    FRONTWALL,
    FLOOR
}

public class MagicalBallScript : MonoBehaviour
{
    public Transform returnPoint;

    public float bounciness;
    public float hitSpeedMultiplier;
    public float magicalBounceSpeed;

    public float initialGravity;
    public float afterRacketArcadeHitGravity;
    public float afterFrontWallBounceGravity;
    public float afterFloorBounceGravity;

    public float maxSpeed;
    public float minSpeed;

    //public bool isResetable; // Nom à changer

    private Rigidbody rigidbody;
    //private Vector3 resetPosition;
    private BallLastInterraction ballInterractionState;


    private Vector3 lastVelocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        //resetPosition = gameObject.transform.position;
        ballInterractionState = BallLastInterraction.NONE;
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)

        if (ballInterractionState == BallLastInterraction.NONE)
            rigidbody.AddForce(initialGravity * Vector3.down);
        else if (ballInterractionState == BallLastInterraction.RACKET)
            rigidbody.AddForce(afterRacketArcadeHitGravity * Vector3.down);
        else if (ballInterractionState == BallLastInterraction.FRONTWALL)
            rigidbody.AddForce(afterFrontWallBounceGravity * Vector3.down);
        else
            rigidbody.AddForce(afterFloorBounceGravity * Vector3.down);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Racket"))
        {
            StartCoroutine(RacketArcadeHit()); 
            ballInterractionState = BallLastInterraction.RACKET;
        }
        else if (other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick"))
        {
            Bounce(other.GetContact(0));
            ballInterractionState = BallLastInterraction.FRONTWALL;
        }
        else if(other.gameObject.CompareTag("Floor"))
        {
            if (ballInterractionState == BallLastInterraction.FRONTWALL)
            {
                MagicalBounce();
                ballInterractionState = BallLastInterraction.FLOOR;
            }
            else
            {
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

    private void MagicalBounce()
    {
        rigidbody.velocity = Vector3.Normalize(returnPoint.position - gameObject.transform.position) * magicalBounceSpeed;
    }

    private void MagicalBounce2()
    {
        rigidbody.velocity = Vector3.Normalize(returnPoint.position - gameObject.transform.position) * magicalBounceSpeed;
    }

    private IEnumerator RacketArcadeHit()
    {
        Transform currentPosition = gameObject.transform;
        GameObject.Find("RacketManager").GetComponent<RacketManager>().OnHitEvent(gameObject);
        yield return new WaitForFixedUpdate();

        Vector3 newVelocity = RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        rigidbody.position = currentPosition.position + newVelocity * Time.fixedDeltaTime * hitSpeedMultiplier;
        rigidbody.velocity = ClampVelocity(newVelocity * hitSpeedMultiplier);
    }

    private Vector3 ClampVelocity(Vector3 velocity)
    {
        if (velocity.magnitude < minSpeed)
        {
            return minSpeed * Vector3.Normalize(velocity);
        }
        else if (velocity.magnitude > maxSpeed)
        {
            return maxSpeed * Vector3.Normalize(velocity);
        }
        else
            return velocity;
    }
}


