using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoLevelPiece : MonoBehaviour
{
#if UNITY_EDITOR
    public enum Category
    {
        Bricks,
        Zones,
    }


    public Category category = Category.Bricks;
    public string itemName = "";
    public Object inspectedScript;
#endif
}
