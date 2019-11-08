using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class EditorUtilityScene 
{
    // Start is called before the first frame update
    public static void NewScene(){
        EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
        //EditorSceneManager.NewScene();
    }

    public static void CleanScene(){
        GameObject[] allObjects = Object.FindObjectsOfType<GameObject>();
        foreach(GameObject go in allObjects){
            GameObject.DestroyImmediate(go);
        }
    }

    public static void NewLevel(){
        NewScene();
        CleanScene();
        GameObject levelGO = new GameObject("Level");
        levelGO.transform.position = Vector3.zero;
        levelGO.AddComponent<LevelScript>();
    }

    public static List<T> GetAssetsWithScript<T> (string path) where T : MonoBehaviour{
        T tmp;
        string assetPath;
        GameObject asset;

        List<T> assetList = new List<T>();
        string[] guids = AssetDatabase.FindAssets("t:Prefab", new string[] {path});
        for(int i = 0; i < guids.Length; i++){
            assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(GameObject)) as GameObject;
            tmp = asset.GetComponent<T>();
            if (tmp != null){
                assetList.Add(tmp);
            }
        }
        return assetList;
    }

    public static List<T> GetListFromEnum<T>(){
        List<T> enumList = new List<T>();
        System.Array enums = System.Enum.GetValues(typeof(T));
        foreach(T e in enums){
            enumList.Add(e);
        }
        return enumList;
    }

    public static T CreateAsset<T>(string path) where T : ScriptableObject{
        T dataClass = (T)ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(dataClass, path);
        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();
        return dataClass;    
    }
}
