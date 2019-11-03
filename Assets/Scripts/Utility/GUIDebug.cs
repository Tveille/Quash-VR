using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIDebug : MonoBehaviour
{
    [SerializeField] string debugText = "DEBUG";

    public void SendDebug()
    {
        Debug.Log(debugText);
    }
}
