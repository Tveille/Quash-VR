using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public float rotSpeed = 10f;
    public Vector3 axis = new Vector3(1, 1, 1);
    public bool debug = false;

    void Update()
    {
        Rotate(Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if(debug)
        Rotate(Time.deltaTime);
    }

    private void Rotate(float delta)
    {
        transform.Rotate(axis * delta * rotSpeed);
    }
}
