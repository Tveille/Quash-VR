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
    public SphereCollider sphereCol;


    private void Awake()
    {
        if (sphereCol == null)
        {
            sphereCol = GetComponent<SphereCollider>();
        }

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
        sphereCol.radius = impactPercent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        BrickManager.Instance.DeadBrick(collision.gameObject, 1);
    }
}
