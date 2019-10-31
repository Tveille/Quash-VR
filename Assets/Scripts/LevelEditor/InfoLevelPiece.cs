using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoLevelPiece : MonoBehaviour
{
    #if UNITY_EDITOR
    public enum Category{
        Misc,
        Colectables,
        Enemies,
        Blocks,
    }

    public Category category = Category.Misc;
    public string itemName = "";
    public Object inspectedScript;
    #endif
}
