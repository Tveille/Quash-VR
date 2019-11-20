using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Malee.Editor;


[CustomEditor(typeof(LevelScript))]
public class LevelInspectorScript : Editor
{
    public enum EditionMode
    {
        Paint2D,
        Erase2D,
    }

    private EditionMode selectedMode;
    private EditionMode currentMode;

    LevelScript myTarget;

    private int newTotalColumns;
    private int newTotalRows;

    private SerializedObject mySerializedObject;

    private InfoLevelPiece itemSelected;
    private Texture2D itemPreview;
    private LevelPiece pieceSelected;
    private GUIStyle titleStyle;

    SerializedProperty editorSpaceProperty;
    ReorderableList mySpace;

    private string levelsPath = "Assets/ScriptableObjects/Levels";
    LevelsScriptable[] levels;
    LevelsScriptable currentLevel;

    WallBuilds walls;
    Wall currentLayer;
    int selectedLayer;
    int numberOfLayers;

    Texture BgTexture;





    public void OnEnable()
    {
        myTarget = (LevelScript)target;

        editorSpaceProperty = serializedObject.FindProperty("editorSpace");
        mySpace = new ReorderableList(editorSpaceProperty, false, false, true);

        BgTexture = EditorGUIUtility.Load("Assets/Graphics/Sprites/Rounded.png") as Texture;


        InitLevel();
        SubscribeEvents();
        InitStyles();

    }

    public void InitStyles()
    {
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 14;

        titleStyle.normal.textColor = new Color(0.145f, 0.58f, .255f, 1f);
    }

    private void OnDisable()
    {
        UnscribeEvents();
    }




    private void SubscribeEvents()
    {
        ObjectWindow.ItemSelectedEvent += new ObjectWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
    }

    private void UnscribeEvents()
    {
        ObjectWindow.ItemSelectedEvent -= new ObjectWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
    }


    private void InitLevel()
    {

        if (AssetDatabase.IsValidFolder("Assets/ScriptableObjects/Levels"))
        {
            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { "Assets/ScriptableObjects/Levels" });
            levels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {

                levels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;

            }

            //System.Array.Reverse(levels);

            //myTarget.allLevels = new LevelsScriptable[levels.Length];
            myTarget.allLevels = levels;

        }
        else
        {
            myTarget.allLevels = new LevelsScriptable[0];
        }

        AssetDatabase.Refresh();
        AssetDatabase.SaveAssets();


        Debug.Log("INIT");


        //myTarget.allLevels = AssetDatabase.FindAssets("t:LevelsScriptable").Length;

        //mySerializedObject = new SerializedObject(myTarget);

        //if (myTarget.Pieces == null || myTarget.Pieces.Length == 0)
        //{
        //    myTarget.Pieces = new LevelPiece[myTarget.TotalRows * myTarget.TotalColumns];
        //}

        // myTarget.transform.hideFlags = HideFlags.NotEditable;
    }





    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawLevelDataGUI();
        DrawPieceSelectedGUI();


        if (GUI.changed)
        {
            EditorUtility.SetDirty(myTarget);
        }
    }

    private void OnSceneGUI()
    {
        DrawLayerGUI();
        DrawLevelGUI();
        DrawModeGUI();
        ModeHandler();
        EventHandler();
    }

    private void DrawLevelDataGUI()
    {
        Undo.RecordObject(myTarget, "Recording Changes");

        EditorGUILayout.LabelField("Grid Parameters", titleStyle);

        EditorGUILayout.BeginVertical("box");

        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField("Selected Level", myTarget.selectedLevel, typeof(LevelsScriptable), false);


        //levelsList.DoLayoutList();

        //myTarget.Settings = (LevelSettings) EditorGUILayout.ObjectField("Level Settings", myTarget.Settings, 
        //typeof(LevelSettings), false);

        if (myTarget.selectedLevel == null)
        {
            EditorGUILayout.HelpBox("Tu dois attacher un level.asset", MessageType.Warning);
        }

        //myTarget.nameLevel = EditorGUILayout.TextField("Name of Level", myTarget.nameLevel);
        //myTarget.background = (Sprite)EditorGUILayout.ObjectField("Background", myTarget.background,
        //                                                                           typeof(Sprite), false);

        myTarget.xGridPlacement = EditorGUILayout.FloatField("Placement de la grille en X", myTarget.xGridPlacement);
        myTarget.yGridPlacement = EditorGUILayout.FloatField("Placement de la grille en Y", myTarget.yGridPlacement);
        myTarget.zGridPlacement = EditorGUILayout.FloatField("Placement de la grille en Z", myTarget.zGridPlacement);

        GUILayout.Space(16);

        EditorGUI.BeginChangeCheck();
        serializedObject.Update();


        mySpace.DoLayoutList();

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {

        }

        GUILayout.Space(16);

        myTarget.CellSize = EditorGUILayout.Slider("Cell Size", myTarget.CellSize, 0.1f, 1f);
        myTarget.TotalRows = (int)EditorGUILayout.Slider("Number of rows", myTarget.TotalRows, 0, (int)(myTarget.maxHeightSpace() / myTarget.CellSize));
        myTarget.TotalColumns = (int)EditorGUILayout.Slider("Number of Columns", myTarget.TotalColumns, 0, (int)(myTarget.maxWidthSpace() / myTarget.CellSize));


        EditorGUILayout.EndHorizontal();
    }

    private void DrawLevelSizeGUI()
    {
        EditorGUILayout.LabelField("Size", titleStyle);

        EditorGUILayout.BeginHorizontal("box");
        EditorGUILayout.BeginVertical();

        //newTotalColumns = EditorGUILayout.IntField("Multiplier", Mathf.Max(1, newTotalColumns));
        //newTotalRows = EditorGUILayout.IntField("Multiplier", Mathf.Max(1, newTotalRows));


        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical();

        bool OldEnabled = GUI.enabled;
        GUI.enabled = (newTotalColumns != myTarget.TotalRows || newTotalRows != myTarget.TotalColumns);

        bool buttonResize = GUILayout.Button("Resize", GUILayout.Height(2 * EditorGUIUtility.singleLineHeight));
        if (buttonResize)
        {
            if (EditorUtility.DisplayDialog(
                "Level Creator",
                "Êtes-vous sûr de vouloir reset le level?\nCette action est irréversible", "Ouais", "Nop"))
            {
                //ResizeLevel();
            }
        }

        bool buttonReset = GUILayout.Button("Reset");



        if (buttonReset)
        {
            //ResetResizeValues();
        }
        GUI.enabled = OldEnabled;

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }

    public void DrawPieceSelectedGUI()
    {
        EditorGUILayout.LabelField("Objet selectionné", titleStyle);
        if (pieceSelected == null)
        {
            EditorGUILayout.HelpBox("Pas d'objet selectionné", MessageType.Info);
        }
        else
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField(new GUIContent(itemPreview), GUILayout.Height(40));
            EditorGUILayout.LabelField(itemSelected.itemName);
            EditorGUILayout.EndVertical();
        }
    }



    public void DrawModeGUI()
    {
        List<EditionMode> modes = EditorUtilityScene.GetListFromEnum<EditionMode>();
        List<string> modeLabels = new List<string>();

        foreach (EditionMode mode in modes)
        {
            modeLabels.Add(mode.ToString());
        }


        Handles.BeginGUI();

        EditorGUI.BeginDisabledGroup(myTarget.selectedLevel == null);

        GUILayout.BeginArea(new Rect(10f, 100, 240, 30f));
        selectedMode = (EditionMode)GUILayout.Toolbar((int)currentMode, modeLabels.ToArray(), GUILayout.ExpandHeight(true));
        GUILayout.EndArea();

        EditorGUI.EndDisabledGroup();

        Handles.EndGUI();
    }


    public void DrawLevelGUI()
    {

        Handles.BeginGUI();

        GUI.Box(new Rect(5, 20, 250, 115), "");

        GUILayout.BeginArea(new Rect(10f, 25, 190, 30));
        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField(myTarget.selectedLevel, typeof(LevelsScriptable), false);
        GUILayout.EndArea();


        if (GUI.Button(new Rect(210, 22.5f, 40, 20), new GUIContent("New", "Create a new Level")))
        {
            string path = "Assets/ScriptableObjects/Levels";

            if (!AssetDatabase.IsValidFolder(path))
            {
                AssetDatabase.CreateFolder("Assets/ScriptableObjects", "Levels");
            }

            LevelsScriptable newLevel = new LevelsScriptable();


            string assetPath = path + "/Level00.asset";
            assetPath = AssetDatabase.GenerateUniqueAssetPath(assetPath);
            AssetDatabase.CreateAsset(newLevel, assetPath);

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();

            EditorGUIUtility.PingObject(newLevel);
            currentLevel = newLevel;
            myTarget.selectedLevel = currentLevel;




            newLevel.level.levelWallBuilds.walls = new Wall[1];



            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { "Assets/ScriptableObjects/Levels" });
            levels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {

                levels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;
                Debug.Log("data path : " + levelsPaths[i]);

            }


            myTarget.allLevels = levels;
        }

        Handles.EndGUI();
    }

    public void DrawLayerGUI()
    {
        Handles.BeginGUI();

        GUIStyle layerStyle = EditorStyles.centeredGreyMiniLabel;
        layerStyle = EditorStyles.boldLabel;

        GUIStyle buttonsStyle = EditorStyles.helpBox;
        //buttonsStyle.fontStyle = ;

        GUI.backgroundColor = Color.white;

        if (GUI.Button(new Rect(55, 50, 20, 18), new GUIContent("<", "previous layer"), buttonsStyle))
        {
            if (selectedLayer < 0)
            {
                selectedLayer--;
            }
        }

        GUILayout.BeginArea(new Rect(10, 50, 190, 30));

        GUI.Label(new Rect(0, 0, 60, 25), "Layer", EditorStyles.boldLabel);



        EditorGUI.BeginChangeCheck();

        selectedLayer = EditorGUI.IntField(new Rect(60, 0, 22, 18), selectedLayer, layerStyle);

        GUI.Label(new Rect(81, 0, 30, 25), "/");

        numberOfLayers = EditorGUI.IntField(new Rect(90, 0, 22, 18), numberOfLayers, layerStyle);


        EditorGUI.EndChangeCheck();

        if (GUI.changed)
        {
            currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];
        }



        GUILayout.EndArea();

        if (GUI.Button(new Rect(121, 50, 20, 18), new GUIContent(">", "next layer"), buttonsStyle))
        {
            if (selectedLayer < numberOfLayers)
            {
                selectedLayer++;
            }
        }

        GUI.backgroundColor = Color.white;
        Handles.EndGUI();
    }

    private void UpdateCurrentPieceInstance(InfoLevelPiece item, Texture2D preview)
    {
        itemSelected = item;
        itemPreview = preview;
        pieceSelected = (LevelPiece)item.GetComponent<LevelPiece>();
        Repaint();
    }

    private void ModeHandler()
    {
        switch (selectedMode)
        {
            case EditionMode.Paint2D:
            case EditionMode.Erase2D:
                Tools.current = Tool.None;
                break;
        }

        //Mode Change
        if (selectedMode != currentMode)
        {
            currentMode = selectedMode;
        }
        //Lock in 2D




    }

    private void EventHandler()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        Camera camera = SceneView.currentDrawingSceneView.camera;

        Vector3 mousePosition = Event.current.mousePosition;
        mousePosition = new Vector2(mousePosition.x, camera.pixelHeight - mousePosition.y);

        Vector3 worldPos = camera.ScreenToWorldPoint(mousePosition);

        Vector3 gridPos = myTarget.WorldToGridCoordinates(worldPos);



        int col = (int)gridPos.x;
        int row = (int)gridPos.y;

        if (myTarget.IsInsideGridBounds(col, row))
        {

        }


        switch (currentMode)
        {
            case EditionMode.Paint2D:
                SceneView _scene = SceneView.lastActiveSceneView;
                _scene.orthographic = true;


                Quaternion newPos = _scene.rotation;

                newPos.x = 0f;
                newPos.y = 0f;
                newPos.z = 0f;
                newPos.w = 0f;

                _scene.rotation = newPos;

                _scene.Repaint();

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    Paint(col, row);
                }
                break;

            case EditionMode.Erase2D:
                SceneView _scene2 = SceneView.lastActiveSceneView;
                _scene2.orthographic = true;

                Quaternion newPos2 = _scene2.rotation;

                newPos.x = 0f;
                newPos.y = 0f;
                newPos.z = 0f;
                newPos.w = 0f;

                _scene2.rotation = newPos;

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    Erase(col, row);
                }
                break;
        }
    }

    private void Paint(int col, int row)
    {

        if (!myTarget.IsInsideGridBounds(col, row) || pieceSelected == null)
        {
            return;
        }


        //if (myTarget.Pieces[col + row * myTarget.TotalRows] != null)
        //{
        //    DestroyImmediate(myTarget.Pieces[col + row * myTarget.TotalRows].gameObject);
        //}

        Debug.LogFormat("GridPos {0},{1}", col, row);


        GameObject obj = PrefabUtility.InstantiatePrefab(pieceSelected.gameObject) as GameObject;

        obj.transform.parent = myTarget.transform;

        obj.name = string.Format("{0},{1},{2}", col, row, obj.name);

        obj.transform.position = myTarget.GridToWorldPoint(col, row);

        //obj.hideFlags = HideFlags.HideInHierarchy;

        //myTarget.Pieces[col + row * myTarget.TotalRows] = obj.GetComponent<LevelPiece>();
    }

    private void Erase(int col, int row)
    {
        if (!myTarget.IsInsideGridBounds(col, row))
        {
            return;
        }

        //if (myTarget.Pieces[col + row * myTarget.TotalRows] != null)
        //{
        //    DestroyImmediate(myTarget.Pieces[col + row * myTarget.TotalRows].gameObject);
        //}
    }

    private void Edit(int col, int row)
    {

    }


}
