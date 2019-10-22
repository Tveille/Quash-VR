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
    public float afterRacketHitGravity;
    public float afterFrontWallBounceGravity;
    public float afterFloorBounceGravity;

    public bool isResetable; // Nom à changer

    private Rigidbody rigidbody;
    private Vector3 resetPosition;
    private BallLastInterraction ballState;


    private Vector3 lastVelocity;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        resetPosition = gameObject.transform.position;
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
            StartCoroutine(Hit()); //Changer le nom de la coroutine
            ballState = BallLastInterraction.RACKET;
        }
        else if ((other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick")) &&  ballState == BallLastInterraction.RACKET)
        {
            Bounce(other.GetContact(0));
            ballState = BallLastInterraction.FRONTWALL;
        }
        else if(other.gameObject.CompareTag("Floor"))
        {
            Debug.Log(ballState);
            if (ballState == BallLastInterraction.FRONTWALL)
            {
                MagicalBounce();
                ballState = BallLastInterraction.FLOOR;
            }
            else if(isResetable)
            {
                Debug.Log("BallBounceDebug1");
                ResetBall();
            }
            else
            {
                Debug.Log("BallBounceDebug2");
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

    private IEnumerator Hit()
    {
        Transform currentPosition = gameObject.transform;
        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnHitEvent(gameObject);
        yield return new WaitForFixedUpdate();

        Vector3 newVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        rigidbody.position = currentPosition.position + newVelocity * Time.fixedDeltaTime * hitSpeedMultiplier;
        rigidbody.velocity = newVelocity * hitSpeedMultiplier;
    }

    public void ResetBall() //Changera avec l'insertion propre des Managers
    {
        ballState = BallLastInterraction.NONE;
        Debug.Log(transform.position);
        Debug.Log(resetPosition);
        transform.position = resetPosition;
        Debug.Log(transform.position);
        rigidbody.velocity = Vector3.zero;
    }
}
