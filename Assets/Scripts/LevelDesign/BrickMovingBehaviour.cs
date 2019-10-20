using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickMovingBehaviour : MonoBehaviour
{
    
    [Header("Wall Layer")]
    public int wallLayerID = 0;

    [Header("Score Modifier")]
    public int scoreValue;

    [Header("Waypoint")]
    [Tooltip("Drop your waypoints here")]
    public List<Waypoint> waypoints;
    private int waypointIndex;

    private Vector3 refVector;
    private bool isWaiting;

    [Header("Pattern")]
    [Tooltip("Is the brick going backward when reaching its last waypoint ?")]
    public bool turningBack;
    private bool onItsWayBack;


    private void Awake()
    {
        if(waypoints.Count != 0)
        {
            this.transform.position = waypoints[waypointIndex].transform.position;

            waypointIndex++;
        }
    }

    private void Update()
    {
        Moving();
    }

    /// <summary>
    /// Déplacement de la Brick
    /// </summary>
    private void Moving()
    {
        this.transform.position = Vector3.SmoothDamp(this.transform.position, waypoints[waypointIndex].transform.position, ref refVector, waypoints[waypointIndex].smoothTime,
            waypoints[waypointIndex].speed);

        if(this.transform.position == waypoints[waypointIndex].transform.position)
        {
            //Debug.Log("Reached");
            if (waypoints[waypointIndex].hasToWait)
            {
                isWaiting = true;
                StartCoroutine(WaitUntil(waypoints[waypointIndex].waitFor));
            }
            else
            {
                NextWaypoint();
            }
        }
    }


    /// <summary>
    /// Définition du prochain 
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
                if (waypointIndex < waypoints.Count - 1)
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
            if (waypointIndex < waypoints.Count - 1)
            {
                waypointIndex++;
            }
            else
            {
                waypointIndex = 0;
            }
        }
    }

    IEnumerator WaitUntil (float timeToWait)
    {
        yield return new WaitForSeconds(timeToWait);

        isWaiting = false;

        NextWaypoint();
    }




    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ball")
        {
            BrickManager.Instance.DeadBrick(this.gameObject, scoreValue);
        }
    }
}
