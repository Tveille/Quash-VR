using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBallInteraction : MonoBehaviour
{
    public float bounciness;
    public float powerMultiplier;
    public float gravity;
    //public float hitfloor;

    private Rigidbody rigidBody;
    private Vector3 lastVelocity;
    
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        lastVelocity = rigidBody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)

        rigidBody.AddForce(new Vector3(0, -gravity, 0));
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Racket"))
            RacketHit(other);
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

        rigidBody.velocity = bounciness * (tangentVelocity * tangent - normalVelocity * normal);
    }

    private void RacketHit(Collision other) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        //Transform currentPosition = gameObject.transform;
        Vector3 racketVelocity = GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;


        Debug.Log(other.contactCount);
        Debug.Log(other.GetContact(0).normal);
        Vector3 contactPointNormal = Vector3.Normalize(other.GetContact(0).normal);
        Debug.Log(contactPointNormal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;

        Vector3 tangentVelocity = relativeVelocity - normalVelocity;        // Ajouter frottement

        rigidBody.velocity = powerMultiplier * (-normalVelocity + tangentVelocity);

        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnHitEvent(gameObject);  // Ignore collision pour quelque frame.
    }
}
