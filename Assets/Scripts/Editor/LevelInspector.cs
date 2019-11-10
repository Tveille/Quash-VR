using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;


[CustomEditor(typeof(LevelsScriptable))]
public class LevelInspector : Editor
{
    ReorderableList myLevelSet;
    SerializedProperty LevelSettingsArray;




    private void OnEnable()
    {
        //LevelSettingsArray = serializedObject.FindProperty("level");

        //myLevelSet = new ReorderableList(LevelSettingsArray);
        //myLevelSet.surrogate = new ReorderableList.Surrogate(typeof(GameObject), AppendObject);
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();


        GUILayout.Space(8);
        //myLevelSet.DoLayoutList();
        //EditorGUILayout.PropertyField(LevelSettingsArray);


        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {

        }
    }

    void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, ReorderableList list)
    {
        //element.FindPropertyRelative("levelWallBuilds"). = ;
        element.FindPropertyRelative("prefab").objectReferenceValue = objectReference;
        element.FindPropertyRelative("size").intValue = 0;
    }
}
