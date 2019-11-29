using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

public class ReorderableListUserInspector : Editor
{
    ReorderableList myList;
    SerializedProperty arrayOfStats;

    void OnEnable()
    {
        arrayOfStats = serializedObject.FindProperty("arrayOfStats");

        // associe la liste à la propriété (array)
        myList = new ReorderableList(serializedObject, arrayOfStats, true, true, true, true);
        // les quatre bools du constructeur correspondent à : draggable, display header, display "add" button, display "remove" button

        // ensuite, la liste marche par callbacks, effectués à chaque action
        myList.drawHeaderCallback = MyListHeader;
        myList.drawElementCallback = MyListElementDrawer;
        myList.onAddCallback += MyListAddCallback;
        myList.onRemoveCallback += MyListRemoveCallback;
        myList.onReorderCallback += (ReorderableList list) => { Debug.Log("la liste vient d'être réordonnée"); };
        myList.elementHeightCallback += (int index) => { return myList.elementHeight * 2; };

        // pour plus d'infos, aller voir la classe sur Github et s'en servir comme documentation :
        // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GUI/ReorderableList.cs
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUILayout.Space(8);
        myList.DoLayoutList(); // affiche la liste (via GUILayout)

        serializedObject.ApplyModifiedProperties();
    }

    void MyListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Voici une liste réordonnable :");
    }

    void MyListElementDrawer(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.yMin += 2;
        rect.yMax -= 4;
        EditorGUI.PropertyField(rect, arrayOfStats.GetArrayElementAtIndex(index), new GUIContent("Couleur "+index.ToString()));
    }

    void MyListAddCallback(ReorderableList rlist)
    {
        arrayOfStats.arraySize++;
        SerializedProperty sp = arrayOfStats.GetArrayElementAtIndex(arrayOfStats.arraySize-1);
        sp.FindPropertyRelative("attack").floatValue = Random.value;
    }

    void MyListRemoveCallback(ReorderableList rlist)
    {
        arrayOfStats.DeleteArrayElementAtIndex(rlist.index);
    }

}