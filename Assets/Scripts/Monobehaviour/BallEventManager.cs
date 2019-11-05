using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallEventManager : MonoBehaviour
{
    public static BallEventManager instance;

    public event Action OnCollisionWithBrick = delegate { };
    public event Action OnCollisionWithFrontWall = delegate { };
    public event Action OnCollisionWithWall = delegate { };
    public event Action OnCollisionWithRacket = delegate { };

    private void Awake()
    {
        instance = this;
    }

    public void OnBallCollision(BallCollisionInfo ballCollisionInfo)
    {
        switch (ballCollisionInfo.OtherTag)
        {
            case "Racket":
                OnCollisionWithRacket();
                break;
            case "Wall":
                OnCollisionWithWall();
                break;
            case "FrontWall":
                OnCollisionWithFrontWall();
                break;
            case "Brick":
                OnCollisionWithBrick();
                break;
            default:
                break;
        }
    }
}

public class BallCollisionInfo
{
    private string otherTag;
    private Vector3 contactPoint;
    private Vector3 contactNormal;
    private Vector3 ballVelocity;

    public string OtherTag { get => otherTag; private set => otherTag = value; }
    public Vector3 ContactPoint { get => contactPoint; private set => contactPoint = value; }
    public Vector3 ContactNormal { get => contactNormal; private set => contactNormal = value; }
    public Vector3 BallVelocity { get => ballVelocity; private set => ballVelocity = value; }

    /// <summary>
    /// Creates a BallCollisioInfo class that contains a large amount of data about the collision of the ball with another object
    /// </summary>
    /// <param name="p_otherTag"> the tag of the other object</param>
    /// <param name="p_contactPoint">the contactPoint</param>
    /// <param name="p_contactNormal">the normal vector of the contact point</param>
    /// <param name="p_ballVelocity">the velocity of the ball when the ball collides with another thing</param>
    public BallCollisionInfo(string p_otherTag,Vector3 p_contactPoint,Vector3 p_contactNormal, Vector3 p_ballVelocity)
    {
        OtherTag = p_otherTag;
        ContactPoint = p_contactPoint;
        ContactNormal = p_contactNormal;
        BallVelocity = p_ballVelocity;
    }

}
