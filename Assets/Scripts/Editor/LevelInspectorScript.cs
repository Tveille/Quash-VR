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
    private float newCellSize;

    private SerializedObject mySerializedObject;

    private InfoLevelPiece itemSelected;
    private Texture2D itemPreview;
    private LevelPiece pieceSelected;

    private GUIStyle titleStyle;
    private GUIStyle layerStyle;
    private GUIStyle buttonsStyle;
    private GUIStyle slashStyle;
    private GUIStyle noneStyle;

    SerializedProperty editorSpaceProperty;
    ReorderableList mySpace;

    private string levelsPath = "Assets/ScriptableObjects/Levels";
    LevelsScriptable[] levels;
    LevelsScriptable currentLevel;

    WallBuilds walls;
    Wall currentLayer;
    int selectedLayer;
    int numberOfLayers;
    int totalLayersDisplayed;




    public void OnEnable()
    {
        myTarget = (LevelScript)target;


        InitGridValues();
        InitSelectedLevelValues();
        GetAllLevels();
        SubscribeEvents();
        InitStyles();
    }


    public void InitGridValues()
    {
        editorSpaceProperty = serializedObject.FindProperty("editorSpace");
        mySpace = new ReorderableList(editorSpaceProperty, false, false, true);

        newCellSize = myTarget.CellSize;
        newTotalColumns = myTarget.TotalColumns;
        newTotalRows = myTarget.TotalRows;
    }

    public void InitSelectedLevelValues()
    {
        if (myTarget.selectedLevel != null)
        {
            currentLevel = myTarget.selectedLevel;
            numberOfLayers = currentLevel.level.levelWallBuilds.walls.Length;
            totalLayersDisplayed = numberOfLayers - 1;
        }
    }

    public void InitStyles()
    {
        //Tittle Style
        titleStyle = new GUIStyle();
        titleStyle.alignment = TextAnchor.MiddleCenter;
        titleStyle.fontSize = 14;
        titleStyle.normal.textColor = new Color(0.145f, 0.58f, .255f, 1f);

        //Layer Label Style
        layerStyle = new GUIStyle();
        layerStyle.fontStyle = FontStyle.Bold;
        layerStyle.alignment = TextAnchor.MiddleCenter;
        layerStyle.normal.textColor = Color.white;

        //Button Style
        buttonsStyle = new GUIStyle();
        buttonsStyle.alignment = TextAnchor.MiddleCenter;
        buttonsStyle.normal.textColor = Color.white;
        buttonsStyle.fontStyle = FontStyle.Bold;
        buttonsStyle.fontSize = 15;

        //Slash Style
        slashStyle = new GUIStyle();
        slashStyle.alignment = TextAnchor.MiddleCenter;
        slashStyle.normal.textColor = Color.white;

        //End of an Option
        noneStyle = new GUIStyle();
        noneStyle.alignment = TextAnchor.MiddleCenter;
        noneStyle.normal.textColor = Color.white;
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




    private void GetAllLevels()
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

            AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
        }
        else
        {
            myTarget.allLevels = new LevelsScriptable[0];
            levels = new LevelsScriptable[0];
        }




        #region Old

        //myTarget.allLevels = AssetDatabase.FindAssets("t:LevelsScriptable").Length;

        //mySerializedObject = new SerializedObject(myTarget);

        //if (myTarget.Pieces == null || myTarget.Pieces.Length == 0)
        //{
        //    myTarget.Pieces = new LevelPiece[myTarget.TotalRows * myTarget.TotalColumns];
        //}

        // myTarget.transform.hideFlags = HideFlags.NotEditable;

        #endregion
    }





    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawLevelDataGUI();

        GUILayout.Space(12);

        DrawLevelSizeGUI();

        GUILayout.Space(12);

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
        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(myTarget, "Recording Changes");

        EditorGUILayout.LabelField("Grid Parameters", titleStyle);

        EditorGUILayout.BeginVertical("box");

        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField("Selected Level", myTarget.selectedLevel, typeof(LevelsScriptable), false);

        GUILayout.Space(8);

        if (myTarget.selectedLevel != null)
            myTarget.levelCategories = myTarget.selectedLevel.level;
        else
            myTarget.levelCategories = null;


        #region Old

        //myTarget.Settings = (LevelSettings) EditorGUILayout.ObjectField("Level Settings", myTarget.Settings, 
        //typeof(LevelSettings), false);

        #endregion

        if (myTarget.selectedLevel == null)
        {
            EditorGUILayout.HelpBox("Tu dois attacher un level.asset", MessageType.Warning);
        }

        GUILayout.Space(8);

        #region Old

        //myTarget.nameLevel = EditorGUILayout.TextField("Name of Level", myTarget.nameLevel);
        //myTarget.background = (Sprite)EditorGUILayout.ObjectField("Background", myTarget.background,
        //                                                                           typeof(Sprite), false);

        #endregion




        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.Update();


            serializedObject.ApplyModifiedProperties();
        }


        EditorGUILayout.EndVertical();
    }

    private void DrawLevelSizeGUI()
    {
        EditorGUILayout.LabelField("Size", titleStyle);

        EditorGUILayout.BeginVertical("box");

        mySpace.DoLayoutList();

        GUILayout.Space(1);

        EditorGUILayout.BeginVertical("box");

        myTarget.xGridPlacement = EditorGUILayout.FloatField("Placement de la grille en X", myTarget.xGridPlacement);
        GUILayout.Space(8);
        myTarget.yGridPlacement = EditorGUILayout.FloatField("Placement de la grille en Y", myTarget.yGridPlacement);
        GUILayout.Space(8);
        myTarget.zGridPlacement = EditorGUILayout.FloatField("Placement de la grille en Z", myTarget.zGridPlacement);

        EditorGUILayout.EndVertical();

        GUILayout.Space(8);

        EditorGUILayout.BeginVertical("box");

        newTotalColumns = (int)EditorGUILayout.Slider("Number of Columns", newTotalColumns, 0, (int)(myTarget.maxWidthSpace() / myTarget.CellSize));
        newTotalRows = (int)EditorGUILayout.Slider("Number of rows", newTotalRows, 0, (int)(myTarget.maxHeightSpace() / myTarget.CellSize));
        newCellSize = EditorGUILayout.Slider("Cell Size", newCellSize, 0.1f, 1f);

        #region Old

        //newTotalColumns = EditorGUILayout.IntField("Multiplier", Mathf.Max(1, newTotalColumns));
        //newTotalRows = EditorGUILayout.IntField("Multiplier", Mathf.Max(1, newTotalRows));

        #endregion

        GUILayout.Space(2);

        bool OldEnabled = GUI.enabled;
        GUI.enabled = (newTotalColumns != myTarget.TotalRows || newTotalRows != myTarget.TotalColumns || newCellSize != myTarget.CellSize);

        bool buttonResize = GUILayout.Button("Resize", GUILayout.Height(2 * EditorGUIUtility.singleLineHeight));
        if (buttonResize)
        {
            if (EditorUtility.DisplayDialog(
                "Level Creator",
                "Êtes-vous sûr de vouloir reset le level?\n" +
                "Cette action est IRREVERSIBLE et a pour effet de REINITIALISER TOUS LES LAYERS",
                "Ouais", "Nop"))
            {
                ResizeLevels();
            }
        }

        GUILayout.Space(2);

        bool buttonReset = GUILayout.Button("Reset Layer Grid");



        if (buttonReset)
        {
            ResetResizeValues();
        }

        GUI.enabled = OldEnabled;

        GUILayout.Space(2);

        EditorGUILayout.EndVertical();
        EditorGUILayout.EndVertical();


    }

    public void DrawPieceSelectedGUI()
    {
        EditorGUILayout.LabelField("Objet selectionné", titleStyle);

        GUILayout.Space(2);

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




    public void DrawLevelGUI()
    {

        Handles.BeginGUI();

        GUI.Box(new Rect(5, 20, 250, 115), "");

        GUILayout.BeginArea(new Rect(10f, 25, 190, 30));
        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField(myTarget.selectedLevel, typeof(LevelsScriptable), false);

        if (myTarget.selectedLevel != null)
            myTarget.levelCategories = myTarget.selectedLevel.level;
        else
            myTarget.levelCategories = null;

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

            newLevel.level.levelWallBuilds = new WallBuilds();
            newLevel.level.levelWallBuilds.walls = new Wall[1];

            currentLevel = newLevel;

            currentLevel.level.levelWallBuilds.walls[0] = new Wall(newTotalColumns * newTotalRows);

            myTarget.selectedLevel = currentLevel;


            numberOfLayers = currentLevel.level.levelWallBuilds.walls.Length;
            totalLayersDisplayed = numberOfLayers - 1;
            selectedLayer = 0;
            currentLayer = currentLevel.level.levelWallBuilds.walls[selectedLayer];



            string[] levelsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { "Assets/ScriptableObjects/Levels" });
            levels = new LevelsScriptable[levelsPaths.Length];

            for (int i = 0; i < levelsPaths.Length; i++)
            {
                levels[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(levelsPaths[i]), typeof(LevelsScriptable)) as LevelsScriptable;
            }


            myTarget.allLevels = levels;
        }

        Handles.EndGUI();
    }



    private void CreateLayer()
    {

    }

    public void DrawLayerGUI()
    {
        Handles.BeginGUI();

        EditorGUI.BeginDisabledGroup(myTarget.selectedLevel == null);

        if (selectedLayer > 0)
        {
            if (GUI.Button(new Rect(55, 43, 26, 23), new GUIContent("<", "Go to Previous Layer"), buttonsStyle))
            {
                selectedLayer--;

                currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];
            }
        }
        else
        {
            if (GUI.Button(new Rect(55, 43, 26, 23), new GUIContent("-", "This is the First Layer"), noneStyle))
            {

            }
        }

        if (selectedLayer < totalLayersDisplayed)
        {
            if (GUI.Button(new Rect(126, 43, 26, 23), new GUIContent(">", "Go to Next Layer"), buttonsStyle))
            {
                selectedLayer++;

                currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];
            }
        }
        else
        {
            //Incrémentation du nombre TOTAL de layer
            if (GUI.Button(new Rect(130, 46, 20, 18), new GUIContent("+", "Add a Layer"), buttonsStyle))
            {
                numberOfLayers++;
                totalLayersDisplayed = numberOfLayers - 1;


                List<Wall> tempWalls = new List<Wall>();

                for (int i = 0; i < numberOfLayers - 1; i++)
                {
                    tempWalls.Add(currentLevel.level.levelWallBuilds.walls[i]);
                }

                currentLevel.level.levelWallBuilds.walls = new Wall[numberOfLayers];

                for (int i = 0; i < numberOfLayers; i++)
                {
                    currentLevel.level.levelWallBuilds.walls[i] = new Wall(newTotalColumns * newTotalRows);
                }


                for (int i = 0; i < numberOfLayers - 1; i++)
                {
                    currentLevel.level.levelWallBuilds.walls[i] = tempWalls[i];
                }

                myTarget.selectedLevel = currentLevel;
            }
        }


        GUI.Label(new Rect(5, 43, 60, 25), "Layer", layerStyle);


        //Changement du LAYER SELECTIONNE
        EditorGUI.BeginChangeCheck();

        selectedLayer = EditorGUI.IntField(new Rect(74, 46, 22, 18), selectedLayer, layerStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if (selectedLayer > totalLayersDisplayed)
            {
                selectedLayer = totalLayersDisplayed;
            }

            currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];
        }

        GUI.Label(new Rect(87, 43, 30, 25), "/", slashStyle);



        //Changement du nombre TOTAL de layer
        EditorGUI.BeginChangeCheck();

        totalLayersDisplayed = EditorGUI.IntField(new Rect(107, 46, 22, 18), totalLayersDisplayed, layerStyle);

        if (EditorGUI.EndChangeCheck())
        {
            int oldNumberOfLayers = myTarget.selectedLevel.level.levelWallBuilds.walls.Length;
            numberOfLayers = totalLayersDisplayed + 1;



            List<Wall> tempWalls = new List<Wall>();

            for (int i = 0; i < oldNumberOfLayers; i++)
            {
                tempWalls.Add(currentLevel.level.levelWallBuilds.walls[i]);
            }


            currentLevel.level.levelWallBuilds.walls = new Wall[numberOfLayers];

            for (int i = 0; i < numberOfLayers; i++)
            {
                currentLevel.level.levelWallBuilds.walls[i] = new Wall(newTotalColumns * newTotalRows);
            }


            for (int i = 0; i < oldNumberOfLayers; i++)
            {
                currentLevel.level.levelWallBuilds.walls[i] = tempWalls[i];
            }


            myTarget.selectedLevel = currentLevel;
        }


        EditorGUI.EndDisabledGroup();


        GUI.backgroundColor = Color.white;
        Handles.EndGUI();
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

        #region Old

        //if (myTarget.Pieces[col + row * myTarget.TotalRows] != null)
        //{
        //    DestroyImmediate(myTarget.Pieces[col + row * myTarget.TotalRows].gameObject);
        //}

        #endregion

        Debug.LogFormat("GridPos {0},{1}", col, row);


        GameObject obj = PrefabUtility.InstantiatePrefab(pieceSelected.gameObject) as GameObject;

        obj.transform.parent = myTarget.transform;

        obj.name = string.Format("{0},{1},{2}", col, row, obj.name);

        obj.transform.position = myTarget.GridToWorldPoint(col, row);

        #region Old

        //obj.hideFlags = HideFlags.HideInHierarchy;

        //myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer].wallBricks[]

        //myTarget.Pieces[col + row * myTarget.TotalRows] = obj.GetComponent<LevelPiece>();

        #endregion
    }

    private void Erase(int col, int row)
    {
        if (!myTarget.IsInsideGridBounds(col, row))
        {
            return;
        }

        #region Old

        //if (myTarget.Pieces[col + row * myTarget.TotalRows] != null)
        //{
        //    DestroyImmediate(myTarget.Pieces[col + row * myTarget.TotalRows].gameObject);
        //}

        #endregion
    }

    private void Edit(int col, int row)
    {

    }

    private void ResetResizeValues()
    {
        newTotalColumns = myTarget.TotalRows;
        newTotalRows = myTarget.TotalColumns;
        newCellSize = myTarget.CellSize;
    }

    private void ResizeLevels()
    {
        ///Reset tous les levels,
        for (int i = 0; i < levels.Length; i++)
        {
            //Tous les murs,
            for (int j = 0; j < levels[i].level.levelWallBuilds.walls.Length; j++)
            {
                //Et la composition du mur ( soit le nombre total de brick possible sur un mur) en fonction des colonnes et des rangées
                //levels[i].level.levelWallBuilds.walls[j].wallBricks = new BrickSettings[newTotalRows * newTotalColumns];
            }
        }

        myTarget.allLevels = levels;
        myTarget.CellSize = newCellSize;
        myTarget.TotalRows = newTotalColumns;
        myTarget.TotalColumns = newTotalRows;

        #region Old
        //LevelPiece[] newPieces = new LevelPiece[newTotalColumns * newTotalRows];

        //for (int col = 0; col < myTarget.TotalRows; col++)
        //{
        //    for (int row = 0; row < myTarget.TotalColumns; row++)
        //    {
        //        if (col < newTotalColumns && row < newTotalRows)
        //        {
        //            newPieces[col + row * newTotalColumns] = myTarget.Pieces[col + row * myTarget.TotalRows];
        //        }
        //        else
        //        {
        //            LevelPiece piece = myTarget.Pieces[col + row * myTarget.TotalRows];
        //            if (piece != null)
        //            {
        //                Object.DestroyImmediate(piece.gameObject);
        //            }
        //        }
        //    }
        //}

        //myTarget.Pieces = newPieces;
        //myTarget.TotalRows = newTotalColumns;
        //myTarget.TotalColumns = newTotalRows;
        #endregion


    }
}
