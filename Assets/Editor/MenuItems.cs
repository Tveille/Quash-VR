using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public static class MenuItems
{
    [MenuItem("Tools/Level Creator/New Level")]
    private static void NewLevel(){
        EditorUtilityScene.NewLevel();
    }

    [MenuItem("Tools/Level Creator/Show Object Window %u")]    
    public static void ShowWindow(){
        ObjectWindow.ShowWindow();
    }

    [MenuItem("Tools/Level Creator/New Level Settings")]
    private static void NewLevelSettings(){
        string path = EditorUtility.SaveFilePanelInProject(
            "New Level Settings", 
            "Level Settings", 
            "Asset", 
            "Nom du LevelSettings");
        if (path != ""){
            EditorUtilityScene.CreateAsset<LevelSettings>(path);
        }
    }
}
