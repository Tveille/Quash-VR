﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelScript))]
public class LevelInspectorScript : Editor
{

    public enum Mode{
        View,
        Paint2D, 
        Erase2D,
    }

    private Mode selectedMode;
    private Mode currentMode;
    LevelScript myTarget;
    private int newTotalColumns;
    private int newTotalRows;
    private SerializedObject mySerializedObject;
    private InfoLevelPiece itemSelected;
    private Texture2D itemPreview;
    private LevelPiece pieceSelected;
    private GUIStyle titleStyle;

    public void OnEnable(){
        myTarget = (LevelScript)target;
        InitLevel();
        ResetResizeValues();
        SubscribeEvents();
        InitStyles();
        
    }

    public void InitStyles(){
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 14;
        
        titleStyle.normal.textColor = new Color(0.145f, 0.58f, .255f, 1f);
    }

    private void OnDisable(){
        UnscribeEvents();
    }

    private void SubscribeEvents(){
        ObjectWindow.ItemSelectedEvent += new ObjectWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
    }

    private void UnscribeEvents(){
        ObjectWindow.ItemSelectedEvent -= new ObjectWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
    }

    private void InitLevel(){
        mySerializedObject = new SerializedObject (myTarget);
        if(myTarget.Pieces == null || myTarget.Pieces.Length == 0){
            myTarget.Pieces = new LevelPiece[myTarget.TotalColumns * myTarget.TotalRows];
        }
        myTarget.transform.hideFlags = HideFlags.NotEditable;
    }

    private void ResetResizeValues(){
        newTotalColumns = myTarget.TotalColumns;
        newTotalRows = myTarget.TotalRows;
    }

    private void ResizeLevel(){
        LevelPiece[] newPieces = new LevelPiece[newTotalColumns * newTotalRows];
        for (int col = 0; col < myTarget.TotalColumns; col++){
            for (int row = 0; row < myTarget.TotalRows; row++){
                if (col < newTotalColumns && row < newTotalRows){
                    newPieces[col + row * newTotalColumns] = myTarget.Pieces[col + row * myTarget.TotalColumns];
                } else {
                    LevelPiece piece = myTarget.Pieces[col + row * myTarget.TotalColumns];
                    if (piece != null){
                        Object.DestroyImmediate(piece.gameObject);
                    }
                }
            }
        }
        myTarget.Pieces = newPieces;
        myTarget.TotalColumns = newTotalColumns;
        myTarget.TotalRows = newTotalRows;
    }


   
    
    public override void OnInspectorGUI(){
       DrawLevelDataGUI();
       DrawLevelSizeGUI();
       DrawPieceSelectedGUI();
        if (GUI.changed){
            EditorUtility.SetDirty(myTarget);
        }
    }

    private void OnSceneGUI(){
        DrawModeGUI();
        ModeHandler();
        EventHandler(); 
    }

    private void DrawLevelDataGUI(){
       EditorGUILayout.LabelField("Data", titleStyle);

        EditorGUILayout.BeginVertical("box");

       myTarget.Settings = (LevelSettings) EditorGUILayout.ObjectField("Level Settings", myTarget.Settings, 
                                                                                    typeof(LevelSettings), false);

       if(myTarget.Settings != null){
           Editor.CreateEditor(myTarget.Settings).OnInspectorGUI();
       } else {
           EditorGUILayout.HelpBox("Tu dois attacher un levelsettings.asset", MessageType.Warning);
       }
       myTarget.nameLevel = EditorGUILayout.TextField("Name of Level", myTarget.nameLevel);
       myTarget.background = (Sprite)EditorGUILayout.ObjectField("Background", myTarget.background, 
                                                                                    typeof(Sprite), false);

        

       EditorGUILayout.EndHorizontal();
    }

    private void DrawLevelSizeGUI(){
        EditorGUILayout.LabelField("Size", titleStyle);

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical();

        //newTotalColumns = EditorGUILayout.IntField("Columns", Mathf.Max(1, newTotalColumns));
        newTotalRows = EditorGUILayout.IntField("Rows", Mathf.Max(1,newTotalRows));

        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        bool OldEnabled = GUI.enabled;
        GUI.enabled = (newTotalColumns != myTarget.TotalColumns || newTotalRows != myTarget.TotalRows);
        bool buttonResize = GUILayout.Button("Resize", GUILayout.Height(2* EditorGUIUtility.singleLineHeight));
        if (buttonResize)
        {
            if(EditorUtility.DisplayDialog(
                "Level Creator",
                "Êtes-vous sûr de vouloir reset le level?\nCette action est irréversible", "Ouais", "Nop")){
                    ResizeLevel();
            }
        }
        
        bool buttonReset = GUILayout.Button("Reset");

       

        if (buttonReset){
            ResetResizeValues();
        }
        GUI.enabled = OldEnabled;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public void DrawPieceSelectedGUI(){
        EditorGUILayout.LabelField("Objet selectionné", titleStyle);
        if(pieceSelected == null){
            EditorGUILayout.HelpBox("Pas d'objet selectionné", MessageType.Info);
        } else {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(new GUIContent(itemPreview), GUILayout.Height(40));
            EditorGUILayout.LabelField(itemSelected.itemName);
            EditorGUILayout.EndVertical();
        }
    }

    

    public void DrawModeGUI(){
        List<Mode> modes = EditorUtilityScene.GetListFromEnum<Mode>();
        List<string> modeLabels = new List<string>();
        foreach(Mode mode in modes){
            modeLabels.Add(mode.ToString());
        }

        Handles.BeginGUI();

        GUILayout.BeginArea(new Rect(10f, 10f, 360, 40f));
        selectedMode = (Mode)GUILayout.Toolbar((int) currentMode, modeLabels.ToArray(), GUILayout.ExpandHeight(true));
        GUILayout.EndArea();
        
        Handles.EndGUI();
    }

    private void UpdateCurrentPieceInstance(InfoLevelPiece item, Texture2D preview){
        itemSelected = item;
        itemPreview = preview;
        pieceSelected = (LevelPiece)item.GetComponent<LevelPiece>();
        Repaint();
    }

    private void ModeHandler(){
        switch(selectedMode){
            case Mode.Paint2D:
            case Mode.Erase2D:
                Tools.current = Tool.None;
                break;
            case Mode.View:
            default:
                Tools.current = Tool.View;
            break;
        }

        //Mode Change
        if (selectedMode != currentMode){
            currentMode = selectedMode;
        }
        //Lock in 2D
         
        

        
    }

    private void EventHandler(){
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Camera camera = SceneView.currentDrawingSceneView.camera;
        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition = new Vector2(mousePosition.x, camera.pixelHeight - mousePosition.y);
        Vector3 worldPos = camera.ScreenToWorldPoint(mousePosition);
        Vector3 gridPos = myTarget.WorldToGridCoordinates(worldPos);
        int col = (int)gridPos.x;
        int row =  (int) gridPos.z;

        if (myTarget.IsInsideGridBounds(col,row)){
            
        }
        

        switch(currentMode){
            case Mode.Paint2D :
            SceneView _scene = SceneView.lastActiveSceneView;
            _scene.orthographic = true;
            
            
            Quaternion newPos = _scene.rotation;
          
            newPos.x = 0.7f;
            newPos.y = 0;
            newPos.z = 0;
            newPos.w = 0.7f;
         
            _scene.rotation = newPos;
        
            _scene.Repaint ();
            
                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) &&Event.current.button == 0){
                    Paint(col, row);
                }
            break;
            case Mode.Erase2D : 
                SceneView _scene2 = SceneView.lastActiveSceneView;
                _scene2.orthographic = true;
                            
                Quaternion newPos2 = _scene2.rotation;
            
                newPos.x = 0.7f;
                newPos.y = 0;
                newPos.z = 0;
                newPos.w = 0.7f;
            
                _scene2.rotation = newPos;

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) &&Event.current.button == 0){
                    Erase(col, row);
                }
            break;
            case Mode.View : 
                default : 
            break;
        }
    }

    private void Paint(int col, int row){
        if(!myTarget.IsInsideGridBounds(col, row) || pieceSelected == null){
           
            return;
        }

        if(myTarget.Pieces[col+ row * myTarget.TotalColumns] != null){
            DestroyImmediate(myTarget.Pieces[col + row * myTarget.TotalColumns].gameObject);
        }
         Debug.LogFormat("GridPos {0},{1}",col, row);
        GameObject obj = PrefabUtility.InstantiatePrefab(pieceSelected.gameObject) as GameObject;
        obj.transform.parent = myTarget.transform;
        obj.name = string.Format("{0},{1},{2}",col,row, obj.name);
        obj.transform.position = myTarget.GridToWorldPoint(col, row);
        obj.hideFlags = HideFlags.HideInHierarchy;
        myTarget.Pieces[col + row * myTarget.TotalColumns] = obj.GetComponent<LevelPiece>();
    }

    private void Erase(int col, int row){
        if(!myTarget.IsInsideGridBounds(col,row)){
            return;
        }

        if (myTarget.Pieces[col+row * myTarget.TotalColumns] != null){
            DestroyImmediate(myTarget.Pieces[col + row * myTarget.TotalColumns].gameObject);
        }
    }

    private void Edit(int col, int row){

    }


}