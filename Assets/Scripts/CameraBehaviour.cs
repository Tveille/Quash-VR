using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public class CameraBehaviour : MonoBehaviour
{
    public float distanceCamPosOffset;
    public float verticalCamPosOffset;
    public float horizontalCamPosOffset;

    public Vector3 targetPos = new Vector3(0f,1.8f,2f);
    public Vector3 diffPos;
    public Vector3 parentPos;

    public Canvas debuggerCanvas;
    private Vector3 initPos;



    private void Awake()
    {
        //initPos = debuggerCanvas.transform.position;
    }

    private void Update()
    {
        //debuggerCanvas.transform.position = initPos - diffPos;

        diffPos = new Vector3(targetPos.x - VRTK_DeviceFinder.HeadsetTransform().transform.localPosition.x, 
            targetPos.y - VRTK_DeviceFinder.HeadsetTransform().transform.localPosition.y, 
            targetPos.z - VRTK_DeviceFinder.HeadsetTransform().transform.localPosition.z);


        VRTK_DeviceFinder.HeadsetTransform().parent.transform.position = parentPos;

        parentPos = new Vector3(diffPos.x + horizontalCamPosOffset,
            diffPos.y + verticalCamPosOffset, diffPos.z + distanceCamPosOffset);

        DebugManager.Instance.DisplayValue(0, ("HeadSet Position" + VRTK_DeviceFinder.HeadsetTransform().localPosition));
        DebugManager.Instance.DisplayValue(1, ("Diff Position" + diffPos));
        DebugManager.Instance.DisplayValue(2, ("Headset Parent Position = " + VRTK_DeviceFinder.HeadsetTransform().parent.transform.localPosition));
        DebugManager.Instance.DisplayValue(3, ("targetPos = " + targetPos));

        distanceCamPosOffset = DebugManager.Instance.Tweak(0, ("distance " + distanceCamPosOffset), distanceCamPosOffset, -0.5f, 4f);
        horizontalCamPosOffset = DebugManager.Instance.Tweak(1, ("horizontal " + horizontalCamPosOffset), horizontalCamPosOffset, -4f, 4f);
    }
}
