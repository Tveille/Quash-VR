using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleBallPhysics : MonoBehaviour
{
    public float bounciness;
    public float velocityMultiplier;
    public float gravity;

    private Rigidbody rigidBody;

    private Vector3 lastVelocity;
    private bool isSubjectToGravity;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        isSubjectToGravity = true;
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidBody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)

        if(isSubjectToGravity)
        {
            rigidBody.AddForce(new Vector3(0, -gravity, 0));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            Bounce(other.GetContact(0));
            isSubjectToGravity = true;
        }
        if (other.gameObject.CompareTag("Racket"))
        {
            //Debug.Log("Collision detected");
            isSubjectToGravity = false;
            StartCoroutine(Hit());
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

        rigidBody.velocity = bounciness * (tangentVelocity * tangent - normalVelocity * normal);
    }

    private IEnumerator Hit()
    {
        Transform currentPosition = gameObject.transform;
        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().HitEvent(gameObject);
        yield return new WaitForFixedUpdate();

        //Debug.Log("Debug Coroutine");
        Vector3 newVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        rigidBody.position = currentPosition.position + newVelocity * Time.fixedDeltaTime * velocityMultiplier;
        rigidBody.velocity = newVelocity * velocityMultiplier;
    }
}
