using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Floatator : MonoBehaviour
{
    [Header("Global settings")]
    [SerializeField] bool beginOnStart = true;
    [SerializeField] bool loop = true;
    [SerializeField] bool randomStart = false;
    [Header("Floating settings")]
    [SerializeField] float floatDuration = 1;
    [SerializeField] Vector3 localTargetPos;
    [SerializeField] AnimationCurve floatingCurve;

    bool canCount;
    float duration = 0.0f;
    Vector3 iniPos;

    private void Start()
    {
        iniPos = transform.localPosition;
        if (randomStart) duration = Random.Range(0.0f, floatDuration);
        canCount = beginOnStart;
    }

    private void Update()
    {
        Count();
    }

    void Count()
    {
        if (!canCount) return;

        duration += Time.deltaTime;
        transform.localPosition = Vector3.Lerp(iniPos, localTargetPos, floatingCurve.Evaluate(duration / floatDuration));
        if (duration >= floatDuration)
        {
            canCount = loop;
            duration = 0;
        }
    }


    public void StartFloat()
    {
        canCount = true;
        duration = 0.0f;
    }

    public void ResumeFloat()
    {
        canCount = true;
    }

    public void StopFloat()
    {
        canCount = false;
    }

}
