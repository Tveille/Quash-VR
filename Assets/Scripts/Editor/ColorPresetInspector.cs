using UnityEngine;
using UnityEditor;
using Malee.Editor;


[CustomEditor(typeof(PresetScriptable))]
public class ColorPresetInspector : Editor
{
    ReorderableList myPresetList;
    SerializedProperty presetsArray;




    private void OnEnable()
    {
        presetsArray = serializedObject.FindProperty("colorPresets");

        myPresetList = new ReorderableList(presetsArray);
        //myPresetList.surrogate = new ReorderableList.Surrogate(typeof(GameObject), AppendObject);
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();


        GUILayout.Space(8);
        myPresetList.DoLayoutList();



        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {

        }
    }

    void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, ReorderableList list)
    {
        element.FindPropertyRelative("tag").stringValue = objectReference.name;
        

        //AssetDatabase.Refresh();
        //AssetDatabase.SaveAssets();
    }
}
