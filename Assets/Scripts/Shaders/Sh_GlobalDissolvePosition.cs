using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sh_GlobalDissolvePosition : MonoBehaviour
{
    public Transform ballTransform;
    void Update()
    {
        Shader.SetGlobalVector("_MagicalBallPos", ballTransform.position);
    }
}
