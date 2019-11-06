using UnityEngine;
using UnityEditor;
using Malee.Editor;


[CustomEditor(typeof(PoolSettings))]
public class PoolInspector : Editor
{
    ReorderableList myPoolList;
    SerializedProperty poolsArray;




    private void OnEnable()
    {
        poolsArray = serializedObject.FindProperty("pools");

        myPoolList = new ReorderableList(poolsArray);
        myPoolList.surrogate = new ReorderableList.Surrogate(typeof(GameObject), AppendObject);
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        serializedObject.Update();


        GUILayout.Space(8);
        myPoolList.DoLayoutList();



        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {

        }
    }

    void AppendObject(SerializedProperty element, UnityEngine.Object objectReference, ReorderableList list)
    {
        element.FindPropertyRelative("tag").stringValue = objectReference.name;
        element.FindPropertyRelative("prefab").objectReferenceValue = objectReference;
        element.FindPropertyRelative("size").intValue = 0;

        //AssetDatabase.Refresh();
        //AssetDatabase.SaveAssets();
    }
}
