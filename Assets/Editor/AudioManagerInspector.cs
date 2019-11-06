using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Malee.Editor;

[CustomEditor(typeof(AudioManager))]
public class AudioManagerInspector : Editor
{
    ReorderableList audioList;
    SerializedProperty amProperty;

    private void OnEnable()
    {
        amProperty = serializedObject.FindProperty("sounds");

        audioList = new ReorderableList(amProperty);
        audioList.surrogate = new ReorderableList.Surrogate(typeof(GameObject), AppendObject);
    }

    void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, ReorderableList list)
    {
        element.FindPropertyRelative("name").stringValue = objectReference.name;
        element.FindPropertyRelative("tag").stringValue = "Activate on Colision with 'tag'";

        element.FindPropertyRelative("clip").objectReferenceValue = objectReference;

        element.FindPropertyRelative("cooldown").floatValue = 0f;

        element.FindPropertyRelative("volume").floatValue = 0f;
        element.FindPropertyRelative("pitch").floatValue = 0.1f;

        element.FindPropertyRelative("loop").boolValue = false;

        element.FindPropertyRelative("spatialBlend").floatValue = 0f;

        element.FindPropertyRelative("panStereo").floatValue = 0f;
        element.FindPropertyRelative("hitPitchRatio").floatValue = 0f;
        element.FindPropertyRelative("minPitch").floatValue = 0f;
        element.FindPropertyRelative("maxPitch").floatValue = 0f;
}

public override void OnInspectorGUI()
{
    EditorGUI.BeginChangeCheck();

    serializedObject.Update();


    GUILayout.Space(8);
    audioList.DoLayoutList();



    serializedObject.ApplyModifiedProperties();

    if (EditorGUI.EndChangeCheck())
    {

    }
}
}
