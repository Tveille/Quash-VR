using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


public class LevelCreatorEditor : Editor
{
    public enum Mode{
        View,
        Paint,
        Edit,
        Erase,
    }

    private Mode selectedMode;
    private Mode currentMode;
}
