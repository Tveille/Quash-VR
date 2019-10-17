using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CameraBehaviour : MonoBehaviour
{
    public float distanceCamPos;
    public float verticalCamPos = 1.7f;
    public float horizontalCamPos;



    private void Update()
    {
        VRTK_DeviceFinder.HeadsetTransform().position = new Vector3(distanceCamPos, verticalCamPos, horizontalCamPos);

        VRTK_DeviceFinder.HeadsetCamera().position = new Vector3(distanceCamPos, verticalCamPos, horizontalCamPos);

        Debug.Log("Headset Transform Position = " + VRTK_DeviceFinder.HeadsetTransform().position);

        Debug.Log("Headset Camera Position = " + VRTK_DeviceFinder.HeadsetCamera().position);
    }
}
