using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BallCollisionFX : MonoBehaviour
{
    public GameObject bounceFXprefab;
    public GameObject Impactprefab;

    public Vector3 fxPosition;
    public Vector3 impactPosition;

    private float currentCooldown = 0f;
    public float maxCooldown;
    private bool canSpawn = false;

    private void Update()
    {
        if (currentCooldown < maxCooldown)
        {
            currentCooldown += Time.deltaTime;
        }
        else
        {
            canSpawn = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            fxPosition = collision.GetContact(0).point;

            Instantiate(bounceFXprefab, fxPosition, collision.gameObject.transform.rotation);
        }

        if (collision.gameObject.tag == "Brick" || collision.gameObject.tag == "FrontWall")
        {
            currentCooldown = 0;

            if (canSpawn)
            {
                impactPosition = collision.GetContact(0).point;

                Instantiate(Impactprefab, impactPosition, Quaternion.identity);

                canSpawn = false;
            }
        }
    }


}
