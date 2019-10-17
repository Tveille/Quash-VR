using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickMovingBehaviour : MonoBehaviour
{
    
    [Header("Wall Layer")]
    public int wallLayerID = 0;

    [Header("Score Modifier")]
    public float scoreValue;

    [Header("Waypoint")]
    public List<Waypoint> waypoints;
    [SerializeField]
    private int waypointIndex;

    private Vector2 refVector;
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
        }
    }

    private void Update()
    {
        Moving();
    }

    private void Moving()
    {
        this.transform.position = Vector2.SmoothDamp(this.transform.position, waypoints[waypointIndex].transform.position, ref refVector, waypoints[waypointIndex].smoothTime,
            waypoints[waypointIndex].speed);

        if(this.transform.position == waypoints[waypointIndex].transform.position)
        {
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

    private void NextWaypoint()
    {
        if (turningBack)
        {
            if (onItsWayBack)
            {
                if (waypointIndex > waypoints.Count)
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
                if (waypointIndex < waypoints.Count)
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
            if (waypointIndex < waypoints.Count)
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
}
