﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;
using Photon.Pun;

public class MagicBallRacketInterractionTests : MonoBehaviourPunCallbacks, IPunObservable
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
        MEDIUMPHYSIC,
        MIXED
    }

    [Header("Racket Settings")]
    public RacketInteractionType physicsUsed;
    public float hitMaxSpeed;
    public float hitMinSpeed;
    public float hitSpeedMultiplier;

    [Header("Racket Physics Settings")]
    public float racketFriction;

    [Header("Racket Mixed Physics Settings")]
    [Range(0, 1)]
    public float mixRatio;

    [Header("Racket Fake Physics Settings")]
    public float maxYAngle;
    public float maxZAngle;
    public float magnitudeSlope;
    public float minMagnitude;
    public float maxMagnitude;

    [Header("Slow Return Settings")]
    public float slowness;

    [Header("Gravity Settings")]
    public float gravity;

    [Header("MagicBounce Depth Settings")]
    public float depthVelocity;
    public Transform zFloorBounceTarget;

    [Header("X Return Settings")]
    public Transform xReturnTarget;

    [Header("Bounce Settings")]
    public float bounciness;
    public float dynamicFriction;

    private Rigidbody rigidbody;

    private BallState ballState;

    private Vector3 lastVelocity;

    PhotonView view;
    Vector3 myVel;
    Vector3 theVel;
    Vector3 ziVel;
    

    void Start()
    {
        view = GetComponent<PhotonView>();
        rigidbody = GetComponent<Rigidbody>();
        ballState = BallState.NORMAL;
    }

    private void FixedUpdate()
    {  
        lastVelocity = rigidbody.velocity;  // Vitesse avant contact necessaire pour le calcul du rebond (méthode Bounce)
        if (ballState == BallState.NORMAL)
            rigidbody.AddForce(gravity * Vector3.down);
        else if (ballState == BallState.SLOW)
            rigidbody.AddForce(gravity / (slowness * slowness) * Vector3.down);
    }

    [PunRPC]
    private void HitUpdate(){
        
    }

    private void OnCollisionEnter(Collision other)
    {
        AudioManager.instance?.PlayHitSound(other.gameObject.tag, other.GetContact(0).point, Quaternion.LookRotation(other.GetContact(0).normal), RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity().magnitude);
        
        if (other.gameObject.CompareTag("Racket") )
        {
            Debug.Log("yes");
            Vector3 newVelocity = Vector3.zero;
            
            switch (physicsUsed)
            {
                case RacketInteractionType.BASICARCADE:

                    newVelocity = RacketArcadeHit();
                    myVel = newVelocity;
                    break;

                case RacketInteractionType.BASICPHYSIC:
                    
                    newVelocity = RacketBasicPhysicHit(other);
                    myVel = newVelocity;
                    break;

                case RacketInteractionType.MEDIUMPHYSIC:
                    
                    newVelocity = RacketMediumPhysicHit(other);
                    myVel = newVelocity;

                    break;
                case RacketInteractionType.MIXED:
        
                    newVelocity = RacketMixedHit(other);
                    myVel = newVelocity;
                    break;
            }

            OnHitCollision(newVelocity);
            view.RPC("OnHitCollision", RpcTarget.Others, myVel);
        }
        else if (other.gameObject.CompareTag("FrontWall") || other.gameObject.CompareTag("Brick"))
        {
            MagicalBounce3(other);
            ballState = BallState.SLOW;
           // view.RPC("MagicalBounce3Velocity", RpcTarget.All, theVel);
        }
        else
        {
            StandardBounce(other.GetContact(0));        // Util?
           // view.RPC("StandardBounceVelocity", RpcTarget.All, ziVel);
        }
                
       
        
        BallEventManager.instance?.OnBallCollision(new BallCollisionInfo(other.gameObject.tag, other.GetContact(0).point, other.GetContact(0).normal,lastVelocity));
    }

    [PunRPC]
    private void OnHitCollision(Vector3 direction){
        myVel = direction  ;
        rigidbody.velocity = ClampVelocity(hitSpeedMultiplier * direction);
        RacketManager.instance.OnHitEvent(gameObject);  // Ignore collision pour quelques frames.
        ballState = BallState.NORMAL;
    }


    //////////////////////////////////    Wall-Floor Interaction     /////////////////////////////////////////////////

    /// Méthode qui calcul le rebond de la balle (calcul vectorielle basique) et modifie la trajectoire en conséquence
    /// contactPoint : données de collision entre la balle et l'autre objet
    
    private void StandardBounce(ContactPoint contactPoint)
    {
        Vector3 normal = Vector3.Normalize(contactPoint.normal);
        float normalVelocity = Vector3.Dot(normal, lastVelocity);
        
        Vector3 tangent = Vector3.Normalize(lastVelocity - normalVelocity * normal);
        float tangentVelocity = Vector3.Dot(tangent, lastVelocity);
        

        rigidbody.velocity = ((1 - dynamicFriction) * tangentVelocity * tangent - bounciness * normalVelocity * normal);
        ziVel = rigidbody.velocity;
        
    }

    private void MagicalBounce3(Collision collision)
    {
        float verticalVelocity = CalculateVerticalBounceVelocity(collision);

        float sideVelocity = CalculateSideBounceVelocity(collision);

        rigidbody.velocity = new Vector3(sideVelocity, verticalVelocity, -depthVelocity) / slowness;
        theVel = rigidbody.velocity;
    }
    [PunRPC]
    private void MagicalBounce3Velocity(Vector3 direction){
        direction = theVel;
    }

    private float CalculateVerticalBounceVelocity(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;
        return (gravity * (zFloorBounceTarget.position.z - transform.position.z) / -depthVelocity / 2) - (transform.position.y * -depthVelocity / (zFloorBounceTarget.position.z - transform.position.z));
    }
    [PunRPC]
    private void StandardBounceVelocity(Vector3 direction){
        direction = ziVel;
    }

    private float CalculateSideBounceVelocity(Collision collision)
    {
        Vector3 collisionPoint = collision.GetContact(0).point;
        Vector3 returnHorizontalDirection = new Vector3(xReturnTarget.position.x - collisionPoint.x, 0, xReturnTarget.position.z - collisionPoint.z);
        returnHorizontalDirection = Vector3.Normalize(returnHorizontalDirection);
        return Vector3.Dot(depthVelocity * Vector3.back, returnHorizontalDirection) * Vector3.Dot(returnHorizontalDirection, Vector3.right);
    }


    //////////////////////////////////////////    Racket Interraction     /////////////////////////////////////////////////

    [PunRPC]
    private Vector3 RacketArcadeHit()
    {
        return RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity();
    }

    [PunRPC]
    private Vector3 RacketBasicPhysicHit(Collision collision)       // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton
        Vector3 relativeVelocity = lastVelocity - racketVelocity;
        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = Vector3.Dot(contactPointNormal, relativeVelocity) * contactPointNormal;
        Vector3 tangentVelocity = (relativeVelocity - normalVelocity) * (1 - racketFriction);        // Ajouter frottement

        return -normalVelocity + tangentVelocity;
    }

    [PunRPC]
    private Vector3 RacketMediumPhysicHit(Collision collision) // Ajout d'un seuil pour pouvoir jouer avec la balle?
    {
        Vector3 racketVelocity = RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity(); // Trés sale! A modifier avec les managers Singleton

        Vector3 contactPointNormal = Vector3.Normalize(collision.GetContact(0).normal);

        Vector3 normalVelocity = (2 * Vector3.Dot(contactPointNormal, racketVelocity) - Vector3.Dot(contactPointNormal, lastVelocity)) * contactPointNormal;
        Vector3 tangentVelocity = (lastVelocity - Vector3.Dot(contactPointNormal, lastVelocity) * contactPointNormal) * (1 - racketFriction);        // Ajouter frottement

        return normalVelocity + tangentVelocity;
    }

    [PunRPC]
    private Vector3 RacketMixedHit(Collision collision)
    {
        return RacketArcadeHit() * (1 - mixRatio) + RacketBasicPhysicHit(collision) * mixRatio;
    }

    private Vector3 RacketFakeHit1(Collision collision)                     //Rotation avec bijection mal définie... Ce sera certainnement pas terrible...
    {
        Vector3 normal = collision.GetContact(0).normal;

        Quaternion hitRotation = Quaternion.FromToRotation(Vector3.forward, normal);
        float yEulerRotation = hitRotation.eulerAngles.y;
        float zEulerRotation = hitRotation.eulerAngles.z;   //Ce serait pas plutôt X qu'on veut?
        
        float newYEulerRotation = (yEulerRotation % 180) * (maxYAngle / 180);
        float newZEulerRotation = (zEulerRotation % 180) * (maxZAngle / 180);

        Vector3 velocityDirection = Vector3.Normalize(new Vector3(Mathf.Cos(newYEulerRotation), 1 / Mathf.Tan(newZEulerRotation), Mathf.Sin(newYEulerRotation)));

        float velocityMagnitude = RacketManager.instance.racket.GetComponent<PhysicInfo>().GetVelocity().magnitude;
        velocityMagnitude *= magnitudeSlope;

        if (velocityMagnitude > maxMagnitude)
            velocityMagnitude = maxMagnitude;
        if (velocityMagnitude < minMagnitude)
            velocityMagnitude = minMagnitude;
        
        return velocityMagnitude * velocityDirection;
    }

    private Vector3 RacketFakeHit2(Collision collision)                     //Rotation avec bijection bien definie!                 Besoin d'un Tool?
    {
        // A Implementer
        return Vector3.zero;
    }

    private Vector3 RacketFakeHit3(Collision collision)                     //VitesseAngulaire probablement pas une bonne idée...
    {
        // A Implementer
        return Vector3.zero;
    }

    //////////////////////////////////////////    Utility Methods     /////////////////////////////////////////////////

    private Vector3 ClampVelocity(Vector3 velocity)        //Nom à modifier
    {
        if (velocity.magnitude < hitMinSpeed)
        {
            return hitMinSpeed * Vector3.Normalize(velocity);
        }
        else if (velocity.magnitude > hitMaxSpeed)
        {
            return hitMaxSpeed * Vector3.Normalize(velocity);
        }
        else
            return velocity;
    }

    private float MakeLinearAssociation(float variable, float slope, float offset)
    {
        return slope * variable + offset;
    }

    #region IPunObservable implementation
    void IPunObservable.OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
{
    if (stream.IsWriting)
    {
        stream.SendNext(transform.position);
        stream.SendNext(transform.rotation);

    }
    else
    {
        transform.position = (Vector3)stream.ReceiveNext();
        transform.rotation = (Quaternion)stream.ReceiveNext();
    }
}
    #endregion
}
