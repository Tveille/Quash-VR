using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class BallCollisionFX : MonoBehaviour
{
    private Vector3 impactPosition;

    private float currentCooldown = 0f;
    public float cooldownBetweenTwoImpactFX;
    private bool canSpawn = false;

    private void Update()
    {
        if (currentCooldown < cooldownBetweenTwoImpactFX)
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
        impactPosition = collision.GetContact(0).point;

        if (collision.gameObject.tag == "Wall")
        {
            PoolManager.instance.SpawnFromPool("BounceFX", impactPosition, collision.gameObject.transform.rotation);
        }

        if (collision.gameObject.tag == "Brick" || collision.gameObject.tag == "FrontWall")
        {
            currentCooldown = 0;

            if (canSpawn)
            {
                PoolManager.instance.SpawnFromPool("ImpactFX", impactPosition, Quaternion.identity);
                canSpawn = false;
            }
        }

        if (collision.gameObject.tag == "Brick")
        {
            BrickManager.Instance.DeadBrick(collision.gameObject, 1);
        }
    }


}
