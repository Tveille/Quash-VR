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
    public List<Transform> psTransform;
    public float maxRadius;
    //public SphereCollider sphereCol;

    public LayerMask layerMask;
    public int numberOfDivision;


    private void Awake()
    {
        //if (sphereCol == null)
        //{
        //    sphereCol = GetComponent<SphereCollider>();
        //}

        for (int i = 0; i < psTransform.Count; i++)
        {
            psTransform[i].localScale = new Vector3(maxRadius, maxRadius, maxRadius);
        }
    }

    private void Update()
    {
        if (impactCurentTime < impactMaxTime)
        {
            impactCurentTime += Time.deltaTime;
        }

        impactPercent = minRadius + ((maxRadius - minRadius) * impactCurve.Evaluate(impactCurentTime));
        //sphereCol.radius = impactPercent;

        //Debug.DrawRay(transform.position, transform.up * impactPercent, Color.green);


        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(transform.position,
                transform.TransformDirection(new Vector3(0f + (1f / (float)numberOfDivision) * j, 1f - (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0f + (1f / (float)numberOfDivision) * j, 1f - (1f / (float)numberOfDivision) * j, -0f)).normalized, out hit,impactPercent, layerMask))
            {
                Debug.Log("HitSomething");

                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }

        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(transform.position,
                transform.TransformDirection(new Vector3(1f - (1f / (float)numberOfDivision) * j, 0f - (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);
            
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(1f - (1f / (float)numberOfDivision) * j, 0f - (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }

        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(transform.position,
                transform.TransformDirection(new Vector3(0f - (1f / (float)numberOfDivision) * j, -1f + (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);
            
            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(0f - (1f / (float)numberOfDivision) * j, -1f + (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }

        for (int j = 0; j < numberOfDivision; j++)
        {
            Debug.DrawRay(transform.position,
                transform.TransformDirection(new Vector3(-1f + (1f / (float)numberOfDivision) * j, 0f + (1f / (float)numberOfDivision) * j, 0f)).normalized * impactPercent, Color.blue);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, transform.TransformDirection(new Vector3(-1f + (1f / (float)numberOfDivision) * j, 0f + (1f / (float)numberOfDivision) * j, 0f)).normalized, out hit,impactPercent, layerMask))
            {
                BrickManager.Instance.DeadBrick(hit.collider.gameObject, 1);
            }
        }





    }

    private void OnCollisionEnter(Collision collision)
    {
        BrickManager.Instance.DeadBrick(collision.gameObject, 1);
    }
}
