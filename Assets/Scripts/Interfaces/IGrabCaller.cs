using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGrabCaller
{
    GrabInfo GetGrabInfo();
    void OnGrab();
    void OnUngrab();
}
