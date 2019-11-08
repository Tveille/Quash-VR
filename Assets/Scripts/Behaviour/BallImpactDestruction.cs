using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallImpactDestruction : MonoBehaviour
{
    [Header("Deflagration Animation")]
    public AnimationCurve impactCurve;
    public float impactMaxTime;
    [SerializeField] private float impactCurentTime;
    private float impactPercent;
    private float minRadius = 0.1f;

    [Header("Deflagration zone")]
    public List<ParticleSystem> ps;
    public float maxRadius;
    //public SphereCollider sphereCol;

    public LayerMask layerMask;
    public int numberOfDivision;
    private float raycastOffset = 0.05f;

    private void Awake()
    {
        impactCurentTime = 0;

        for (int i = 0; i < ps.Count; i++)
        {
            ps[i].transform.localScale = new Vector3(maxRadius, maxRadius, maxRadius);
        }
    }


    private void Update()
    {
        if (impactCurentTime < impactMaxTime)
        {
            impactCurentTime += Time.deltaTime;
        }
        else
        {
            impactCurentTime = 0;

            this.gameObject.SetActive(false);
        }

        impactPercent = minRadius + ((maxRadius - minRadius) * impactCurve.Evaluate(impactCurentTime));

        Vector3 originPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + raycastOffset);

        //RaycastHit hitback;

        //Debug.DrawRay(originPos,
        //        transform.TransformDirection(transform.forward).normalized * 0.1f, Color.green);

        //if (Physics.Raycast(originPos, transform.TransformDirection(transform.forward).normalized, out hitback, 0.1f, layerMask))
        //{
        //    Debug.Log("HitSomething");

        //    BrickManager.Instance.DeadBrick(hitback.collider.gameObject, 1);
        //}


        #region MyRaycast
        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPos,
                transform.TransformDirection(new Vector3(0f + (1f / (float)numberOfDivision) * j, 1f - (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(0f + (1f / (float)numberOfDivision) * j, 1f - (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }

        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPos,
                transform.TransformDirection(new Vector3(1f - (1f / (float)numberOfDivision) * j, 0f - (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);
            
            RaycastHit hit;

            if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(1f - (1f / (float)numberOfDivision) * j, 0f - (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }

        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPos,
                transform.TransformDirection(new Vector3(0f - (1f / (float)numberOfDivision) * j, -1f + (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);
            
            RaycastHit hit;

            if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(0f - (1f / (float)numberOfDivision) * j, -1f + (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }

        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(originPos,
                transform.TransformDirection(new Vector3(-1f + (1f / (float)numberOfDivision) * j, 0f + (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(originPos, transform.TransformDirection(new Vector3(-1f + (1f / (float)numberOfDivision) * j, 0f + (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }
        #endregion
    }

}
