using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehaviours : MonoBehaviour
{

    [Header("Score Modifier")]
    public int scoreValue;


    [Header("Armor")]
    [Tooltip("How many hit BEFORE the next one kill it")]
    public int armorPoints = 1;


    [Header("Waypoint")]
    public bool isMoving;
    [Tooltip("Enter waypoints positions here")]
    public Vector3[] waypoints;

    private int waypointIndex;
    private Vector3 refVector;

    [Header("Waiting Parameters")]
    public bool hasToWait;
    public float waitFor;

    [Header("Move Modifiers")]
    [Range(0, 1)]
    [Tooltip("Damping strength")]
    public float smoothTime;
    [Tooltip("Speed of the brick")]
    [Range(0.1f, 10)]
    public float speed;
    private bool isWaiting;

    [Header("Pattern")]
    [Tooltip("Is the brick going backward when reaching its last waypoint ?")]
    public bool turningBack;
    private bool onItsWayBack;





    private void Awake()
    {
        if (isMoving)
        {
            if (waypoints.Length != 0)
            {
                this.transform.position = waypoints[waypointIndex];

                waypointIndex++;
            }
        }
    }


    private void Update()
    {
        if (isMoving)
        {
            Moving();
        }
    }


    private void Moving()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, waypoints[waypointIndex], ref refVector, smoothTime,
            speed);

        if (this.transform.position == waypoints[waypointIndex])
        {
            //Debug.Log("Reached");
            if (hasToWait)
            {
                isWaiting = true;
                StartCoroutine(WaitUntil(waitFor));
            }
            else
            {
                NextWaypoint();
            }
        }
    }

    /// <summary>
    /// Définition du prochain waypoint
    /// </summary>
    private void NextWaypoint()
    {
        if (turningBack)
        {
            if (onItsWayBack)
            {
                if (waypointIndex > 0)
                {
                    waypointIndex--;
                }
                else
                {
                    onItsWayBack = false;
                    waypointIndex++;
                }
            }
            else
            {
                if (waypointIndex < waypoints.Length - 1)
                {
                    waypointIndex++;
                }
                else
                {
                    onItsWayBack = true;
                    waypointIndex--;
                }
            }
        }
        else
        {
            if (waypointIndex < waypoints.Length - 1)
            {
                waypointIndex++;
            }
            else
            {
                waypointIndex = 0;
            }
        }
    }

    IEnumerator WaitUntil(float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        isWaiting = false;

        NextWaypoint();
    }



}
