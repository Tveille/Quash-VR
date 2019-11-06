using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRacketBehaviour : MonoBehaviour
{
    public float returnDuration;

    private Rigidbody rigidbody;
    private float returnStartingTime;

    private void Start()
    {
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    public IEnumerator RacketCallBack(PlayerID userID)
    {
        returnStartingTime = Time.time;

        while(true)     // Condition à modifier
        {
            
            yield return new WaitForFixedUpdate();
        }
        // A remplir
        
    }
}
