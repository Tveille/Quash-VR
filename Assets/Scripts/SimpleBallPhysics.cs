using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBallPhysics : MonoBehaviour
{
    public float bounciness;

    private Rigidbody rigidbody;
    private Vector3 lastVelocity;

    private float lastHitTime;
    private float velocityMultiplier = 1.1f;
    private float deltaHitTime = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        lastHitTime = Time.time - deltaHitTime;
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)

        if (!GameObject.FindWithTag("Racket").GetComponent<BoxCollider>().enabled && ((Time.time - lastHitTime) > deltaHitTime))
        {
            GameObject.FindWithTag("Racket").GetComponent<BoxCollider>().enabled = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Bounce(other.GetContact(0));
        }
        if (other.gameObject.CompareTag("Racket"))
        {
            lastHitTime = Time.time;
            Debug.Log("Collision detected");
            StartCoroutine(Hit());
            GameObject.FindWithTag("Racket").GetComponent<BoxCollider>().enabled = false;
        }
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

    private IEnumerator Hit()
    {
        Transform currentPosition = gameObject.transform;
        yield return new WaitForFixedUpdate();
        Debug.Log("Debug Coroutine");
        Vector3 newVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        rigidbody.position = currentPosition.position + newVelocity * Time.fixedDeltaTime * velocityMultiplier;
        rigidbody.velocity = newVelocity * velocityMultiplier;
    }
}
