using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacketInterractionCodeSamples : MonoBehaviour
{
    //private void RacketArcadeHit()
    //{
    //    Transform currentPosition = gameObject.transform;
    //    GameObject.Find("RacketManager").GetComponent<RacketManager>().OnHitEvent(gameObject);


    //    Vector3 newVelocity = GameObject.Find("RacketManager").GetComponent<RacketManager>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

    //    newVelocity = ClampVelocity(newVelocity * hitSpeedMultiplier);
    //    GetComponent<Rigidbody>().position = currentPosition.position + newVelocity * Time.fixedDeltaTime;
    //    GetComponent<Rigidbody>().velocity = newVelocity;
    //}

    //private void RacketBasicPhysicHit(Collision other) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    //{
    //    Vector3 racketVelocity = GameObject.Find("RacketManager").GetComponent<RacketManager>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
    //    Vector3 relativeVelocity = lastVelocity - racketVelocity;
    //    Vector3 contactPointNormal = Vector3.Normalize(other.GetContact(0).normal);

    //    Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;
    //    Vector3 tangentVelocity = relativeVelocity - normalVelocity;        // Ajouter frottement

    //    GetComponent<Rigidbody>().velocity = ClampVelocity(hitSpeedMultiplier * (-normalVelocity + tangentVelocity));

    //    GameObject.Find("RacketManager").GetComponent<RacketManager>().OnHitEvent(gameObject);  // Ignore collision pour quelque frame.
    //}

    //private void RacketMediumPhysicHit(Collision other) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    //{
    //    Vector3 racketVelocity = GameObject.Find("RacketManager").GetComponent<RacketManager>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
    //    Vector3 relativeVelocity = lastVelocity - racketVelocity;
    //    Vector3 contactPointNormal = Vector3.Normalize(other.GetContact(0).normal);

    //    Vector3 normalVelocity = (2 * Vector3.Dot(contactPointNormal, racketVelocity) - Vector3.Dot(contactPointNormal, lastVelocity)) * hitSpeedMultiplier * contactPointNormal;
    //    Vector3 tangentVelocity = (lastVelocity - Vector3.Dot(contactPointNormal, lastVelocity) * contactPointNormal) * (1 - frottementRacket);        // Ajouter frottement

    //    GetComponent<Rigidbody>().velocity = ClampVelocity(hitSpeedMultiplier * (normalVelocity + tangentVelocity));

    //    GameObject.Find("RacketManager").GetComponent<RacketManager>().OnHitEvent(gameObject);  // Ignore collision pour quelque frame.
    //}
}
