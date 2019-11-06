using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;



[CreateAssetMenu(fileName = "Pool List.asset", menuName = "Custom/Pool List", order = 100)]
public class PoolSettings : ScriptableObject
{
    public Pool[] pools = new Pool[1];
}


[System.Serializable]
public struct Pool
{
    public string tag;
    public GameObject prefab;
    public int size;
}


