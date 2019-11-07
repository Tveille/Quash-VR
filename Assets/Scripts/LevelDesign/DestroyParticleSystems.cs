using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystems : MonoBehaviour
{
    public List<ParticleSystem> particles;

    void Awake()
    {
        InvokeRepeating("CheckParticleSystem", 0.1f, 0.1f);
    }


    private void CheckParticleSystem()
    {
        bool isEverythingDead = false;

        for (int i = 0; i < particles.Count; i++)
        {
            isEverythingDead = true;

            if (particles[i].IsAlive())
            {
                isEverythingDead = false;
            }
        }

        if (isEverythingDead)
        {
            this.gameObject.SetActive(false);
        }
    }
}
