using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;


[CustomEditor(typeof(SoundClass))]
public class SoundClassInspector : Editor
{
    Malee.Editor.ReorderableList audioList;
    SerializedProperty soundsProperty;


    private void OnEnable()
    {
        soundsProperty = serializedObject.FindProperty("sounds");

        audioList = new Malee.Editor.ReorderableList(soundsProperty);
        //audioList.surrogate = new Malee.Editor.ReorderableList.Surrogate(typeof(GameObject), AppendObject);
    }

    void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, Malee.Editor.ReorderableList list)
    {
        element.FindPropertyRelative("name").stringValue = objectReference.name;
        element.FindPropertyRelative("clip").objectReferenceValue = objectReference;
    }


    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();


        GUILayout.Space(8);
        audioList.DoLayoutList();

        GUILayout.Space(50);

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {

        }


    }
}
