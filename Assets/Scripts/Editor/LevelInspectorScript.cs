using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using Malee.Editor;
using UnityEditorInternal;


[CustomEditor(typeof(LevelScript))]
public class LevelInspectorScript : Editor
{
    public enum EditionMode
    {
        Paint2D,
        Erase2D,
        Select,
        View,
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
    Malee.Editor.ReorderableList mySpace;

    private string levelsPath = "Assets/ScriptableObjects/Levels";
    LevelsScriptable[] levels;
    LevelsScriptable currentLevel;



    WallBuilds walls;
    Wall currentLayer;
    int selectedLayer;
    int numberOfLayers;
    int totalLayersDisplayed;

    private GameObject prefabBase;
    private string prefabPath = "Assets/Prefabs/Bricks";

    private PresetScriptable[] colorPresets;
    private string presetPath = "Assets/ScriptableObjects/ColorPresets";

    public enum PaintMode { OnDraw, OnSelection }
    PaintMode paintMode;
    private string[] paintModeLabel;
    BrickSettings paintedBrickSettings;

    bool nameChanged;

    SerializedProperty paintedBrickSet;
    UnityEditorInternal.ReorderableList waypointStorageList;



    public void OnEnable()
    {
        myTarget = (LevelScript)target;

        myTarget.bricksOnScreen = new GameObject[myTarget.TotalColumns * myTarget.TotalRows];

        prefabBase = EditorUtilityScene.GetAssetsWithScript<BrickBehaviours>(prefabPath)[0].gameObject;

        prefabBase = myTarget.brickPrefab;


        InitColorPresets();
        InitGridValues();
        InitReorderableList();
        InitSelectedLevelValues();
        GetAllLevels();
        //SubscribeEvents();
        InitEditModes();
        InitStyles();
    }

    private void OnDisable()
    {
        //UnscribeEvents();
        CleanLayer();
    }




    void InitReorderableList()
    {
        // les quatre bools du constructeur correspondent à : draggable, display header, display "add" button, display "remove" button
        paintedBrickSet = serializedObject.FindProperty("waypointsToPaint");
        waypointStorageList = new UnityEditorInternal.ReorderableList(serializedObject, paintedBrickSet, true, true, true, true);

        // ensuite, la liste marche par callbacks, effectués à chaque action
        waypointStorageList.drawHeaderCallback = MyListHeader;
        waypointStorageList.drawElementCallback = MyListElementDrawer;
        waypointStorageList.onAddCallback += MyListAddCallback;
        waypointStorageList.onRemoveCallback += MyListRemoveCallback;
        waypointStorageList.onReorderCallback += (UnityEditorInternal.ReorderableList list) => { Debug.Log("la liste vient d'être réordonnée"); };
    }

    #region Reorderlist Stuff
    void MyListHeader(Rect rect)
    {
        EditorGUI.LabelField(rect, "Waypoints");
    }

    void MyListElementDrawer(Rect rect, int index, bool isActive, bool isFocused)
    {
        rect.yMin += 2;
        rect.yMax -= 4;
        EditorGUI.PropertyField(rect, paintedBrickSet.GetArrayElementAtIndex(index), new GUIContent("Waypoint " + index.ToString()));
    }

    void MyListAddCallback(UnityEditorInternal.ReorderableList rlist)
    {
        paintedBrickSet.arraySize++;
        SerializedProperty sp = paintedBrickSet.GetArrayElementAtIndex(paintedBrickSet.arraySize - 1);
        sp.vector3Value = new Vector3(0, 0, 0);
    }

    void MyListRemoveCallback(UnityEditorInternal.ReorderableList rlist)
    {
        paintedBrickSet.DeleteArrayElementAtIndex(rlist.index);
    }
    #endregion



    private void InitColorPresets()
    {
        if (AssetDatabase.IsValidFolder(presetPath))
        {
            string[] presetsPaths = AssetDatabase.FindAssets("t:scriptableobject", new[] { presetPath });
            colorPresets = new PresetScriptable[presetsPaths.Length];

            for (int i = 0; i < presetsPaths.Length; i++)
            {

                colorPresets[i] = AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(presetsPaths[i]), typeof(PresetScriptable)) as PresetScriptable;

            }

            myTarget.ColorPreset = colorPresets;
        }
        else
        {
            myTarget.ColorPreset = new PresetScriptable[0];
            colorPresets = new PresetScriptable[0];
        }
    }

    public void InitGridValues()
    {
        editorSpaceProperty = serializedObject.FindProperty("editorSpace");
        mySpace = new Malee.Editor.ReorderableList(editorSpaceProperty, false, false, true);


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
            selectedLayer = 0;
            myTarget.bricksOnLayer = 0;

            SpawnLayer();
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

    private void InitEditModes()
    {
        paintModeLabel = new string[2] { "onDraw", "onSelection" };
    }





    //private void SubscribeEvents()
    //{
    //    ObjectWindow.ItemSelectedEvent += new ObjectWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
    //}

    //private void UnscribeEvents()
    //{
    //    ObjectWindow.ItemSelectedEvent -= new ObjectWindow.itemSelectedDelegate(UpdateCurrentPieceInstance);
    //}




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

        DrawLevelDataInspecGUI();

        GUILayout.Space(12);

        DrawLevelSizeInspecGUI();

        //GUILayout.Space(12);

        //DrawPieceSelectedGUI();

        GUILayout.Space(12);

        DrawEditModesInspecGUI();

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




    private void DrawLevelDataInspecGUI()
    {
        EditorGUI.BeginChangeCheck();

        Undo.RecordObject(myTarget, "Recording Changes");

        EditorGUILayout.LabelField("Grid Parameters", titleStyle);

        GUILayout.Space(16);

        EditorGUILayout.BeginVertical("box");

        EditorGUI.BeginChangeCheck();

        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField("Selected Level", myTarget.selectedLevel, typeof(LevelsScriptable), false);

        if (EditorGUI.EndChangeCheck())
        {
            if (myTarget.selectedLevel != null)
            {
                InitSelectedLevelValues();
                Undo.RecordObject(myTarget.selectedLevel, "Recording Selected Level Choice");
            }
        }




        if (myTarget.selectedLevel != null)
        {
            myTarget.levelCategories = myTarget.selectedLevel.level;


            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal("box");

            myTarget.selectedLevel.name = EditorGUILayout.TextField("Level Name", myTarget.selectedLevel.name);

            if (EditorGUI.EndChangeCheck())
            {
                nameChanged = true;
                Undo.RecordObject(myTarget.selectedLevel, "Recording Selected Name");
            }

            if (nameChanged)
            {
                if (GUILayout.Button("Save"))
                {
                    string assetPath = AssetDatabase.GetAssetPath(myTarget.selectedLevel);
                    AssetDatabase.RenameAsset(assetPath, myTarget.selectedLevel.name);
                    Undo.RecordObject(myTarget.selectedLevel, "Recording Selected Name");
                    //Undo.RecordObject(this, "Recording Selected Name");

                    nameChanged = false;
                }
            }

            EditorGUILayout.EndHorizontal();

        }
        else
            myTarget.levelCategories = null;


        if (myTarget.selectedLevel == null)
        {
            EditorGUILayout.HelpBox("Tu dois attacher un level.asset", MessageType.Warning);
        }

        GUILayout.Space(2);


        EditorGUILayout.EndVertical();
    }

    private void DrawLevelSizeInspecGUI()
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

    //public void DrawPieceSelectedGUI()
    //{
    //    EditorGUILayout.LabelField("Objet selectionné", titleStyle);

    //    GUILayout.Space(2);

    //    if (prefabBase == null)
    //    {
    //        EditorGUILayout.HelpBox("Pas d'objet selectionné", MessageType.Info);
    //    }
    //    else
    //    {
    //        EditorGUILayout.BeginVertical("box");
    //        //EditorGUILayout.LabelField(new GUIContent(itemPreview), GUILayout.Height(40));
    //        //EditorGUILayout.LabelField(itemSelected.itemName);
    //        EditorGUILayout.EndVertical();
    //    }
    //}

    private void DrawEditModesInspecGUI()
    {
        EditorGUILayout.LabelField("Paint Parameters", titleStyle);

        GUILayout.Space(2);

        int index = (int)paintMode;
        index = GUILayout.Toolbar(index, paintModeLabel);

        paintMode = (PaintMode)index;


        switch (paintMode)
        {
            case PaintMode.OnDraw:
                {
                    GUILayout.BeginVertical("box");

                    paintedBrickSettings.brickColorPreset = EditorGUILayout.IntSlider("Color Preset", paintedBrickSettings.brickColorPreset, 0, myTarget.ColorPreset[0].colorPresets.Length - 1);

                    GUILayout.BeginHorizontal("box");

                    EditorGUILayout.LabelField(myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].tag);

                    EditorGUILayout.ColorField(myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].fresnelColors);
                    EditorGUILayout.ColorField(myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].coreEmissiveColors);

                    GUILayout.EndHorizontal();

                    GUILayout.Space(8);

                    GUILayout.BeginVertical("box");
                    paintedBrickSettings.armorValue = EditorGUILayout.IntField("Armor Value", paintedBrickSettings.armorValue);
                    paintedBrickSettings.scoreValue = EditorGUILayout.IntField("Score Value", paintedBrickSettings.scoreValue);

                    GUILayout.Space(8);

                    GUILayout.BeginVertical("box");

                    paintedBrickSettings.isMoving = EditorGUILayout.ToggleLeft("Is the Brick Moving ?", paintedBrickSettings.isMoving);

                    if (paintedBrickSettings.isMoving)
                    {
                        //paintedBrickSettings.isMoving = EditorGUILayout.Foldout(paintedBrickSettings.isMoving, "Parameters");
                        paintedBrickSettings.smoothTime = EditorGUILayout.Slider("smoothTime", paintedBrickSettings.smoothTime, 0f, 1f);
                        paintedBrickSettings.speed = EditorGUILayout.Slider("speed", paintedBrickSettings.speed, 0.1f, 10f);

                        serializedObject.Update();

                        //EditorGUILayout.PropertyField(paintedBrickSet);
                        waypointStorageList.DoLayoutList();
                        paintedBrickSettings.waypointsStorage = myTarget.waypointsToPaint;

                        serializedObject.ApplyModifiedProperties();
                    }


                    GUILayout.EndVertical();
                    GUILayout.EndVertical();
                    GUILayout.EndVertical();

                    break;
                }

            case PaintMode.OnSelection:
                {
                    GUILayout.BeginVertical("box");

                    /*
                    if (Selection.activeObject != null)
                    {
                        GameObject brickSelected = Selection.activeObject as GameObject;
                        BrickBehaviours scriptSelected = brickSelected.GetComponent<BrickBehaviours>();
                        //BrickSettings currentBrickSelected = brickSelected.GetComponent<BrickBehaviours>();

                        scriptSelected.armorPoints = EditorGUILayout.IntField("Armor Value", paintedBrickSettings.armorValue);
                        scriptSelected.scoreValue = EditorGUILayout.IntField("Score Value", paintedBrickSettings.scoreValue);


                        scriptSelected.isMoving = EditorGUILayout.ToggleLeft("Is the Brick Moving ?", paintedBrickSettings.isMoving);

                        if (scriptSelected.isMoving)
                        {
                            scriptSelected.isMoving = EditorGUILayout.Foldout(paintedBrickSettings.isMoving, "Parameters");
                            scriptSelected.smoothTime = EditorGUILayout.Slider(paintedBrickSettings.smoothTime, 0f, 1f);
                            scriptSelected.speed = EditorGUILayout.Slider(paintedBrickSettings.speed, 0.1f, 10f);

                            serializedObject.Update();

                            //EditorGUILayout.PropertyField(paintedBrickSet);
                            myTarget.waypointsToPaint = scriptSelected.waypoints;
                            waypointStorageList.DoLayoutList();
                            scriptSelected.waypoints = myTarget.waypointsToPaint;

                            serializedObject.ApplyModifiedProperties();
                        }
                    }
                    */
                    GUILayout.EndVertical();
                    break;
                }
        }
    }

    public void DrawLevelGUI()
    {

        Handles.BeginGUI();

        GUI.Box(new Rect(5, 20, 250, 115), "");

        GUILayout.BeginArea(new Rect(10f, 25, 190, 30));

        EditorGUI.BeginChangeCheck();

        myTarget.selectedLevel = (LevelsScriptable)EditorGUILayout.ObjectField(myTarget.selectedLevel, typeof(LevelsScriptable), false);

        if (EditorGUI.EndChangeCheck())
        {
            if (myTarget.selectedLevel != null)
            {
                InitSelectedLevelValues();
            }
        }

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

            CleanLayer();
            SpawnLayer();
        }

        Handles.EndGUI();
    }



    private void ResetLayer()
    {
        for (int i = 0; i < myTarget.bricksOnScreen.Length; i++)
        {
            if (myTarget.bricksOnScreen[i] != null)
            {
                DestroyImmediate(myTarget.bricksOnScreen[i], false);
                BrickSettings blankBrick = new BrickSettings();
                currentLayer.wallBricks[i] = blankBrick;
            }
        }
    }

    private void CleanLayer()
    {
        if (currentLayer != null)
        {
            for (int i = 0; i < myTarget.bricksOnScreen.Length; i++)
            {
                if (myTarget.bricksOnScreen[i] != null)
                {
                    DestroyImmediate(myTarget.bricksOnScreen[i], false);
                }
            }


            myTarget.bricksOnScreen = new GameObject[myTarget.TotalColumns * myTarget.TotalRows];
        }
    }

    private void SpawnLayer()
    {
        currentLayer = currentLevel.level.levelWallBuilds.walls[selectedLayer];

        for (int i = 0; i < currentLayer.wallBricks.Count; i++)
        {
            if (currentLayer.wallBricks[i].isBrickHere)
            {
                //GameObject obj = PrefabUtility.InstantiatePrefab(prefabBase) as GameObject;
                GameObject obj = Instantiate(prefabBase) as GameObject;
                BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
                MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
                Material[] mats = objMesh.sharedMaterials;

                mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
                mats[1].SetFloat("_Metallic", 0.75f);

                mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
                mats[0].SetColor("_FresnelColor", myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].fresnelColors);
                mats[0].SetColor("_CoreEmissiveColor", myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].coreEmissiveColors);
                mats[0].SetFloat("_XFrameThickness", 0.75f);
                mats[0].SetFloat("_YFrameThickness", 0.75f);

                objMesh.sharedMaterials = mats;



                //mats[0].SetColor("", );

                obj.transform.parent = myTarget.transform;

                obj.name = currentLayer.wallBricks[i].brickID;

                obj.transform.position = currentLayer.wallBricks[i].brickPosition;



                objBehaviours.armorPoints = currentLayer.wallBricks[i].armorValue;
                objBehaviours.scoreValue = currentLayer.wallBricks[i].scoreValue;

                if (currentLayer.wallBricks[i].isMoving)
                {
                    objBehaviours.isMoving = currentLayer.wallBricks[i].isMoving;
                    objBehaviours.speed = currentLayer.wallBricks[i].speed;
                    objBehaviours.smoothTime = currentLayer.wallBricks[i].smoothTime;
                    objBehaviours.waypoints = new Vector3[currentLayer.wallBricks[i].waypointsStorage.Length];

                    for (int j = 0; j < currentLayer.wallBricks[i].waypointsStorage.Length; j++)
                    {
                        objBehaviours.waypoints[j] = currentLayer.wallBricks[i].waypointsStorage[j];
                    }
                }


                myTarget.bricksOnLayer++;

                myTarget.bricksOnScreen[i] = obj;
            }
        }
    }




    public void DrawLayerGUI()
    {
        Handles.BeginGUI();

        EditorGUI.BeginDisabledGroup(myTarget.selectedLevel == null);

        if (selectedLayer > 0)
        {
            //Passage au layer PRECEDENT
            if (GUI.Button(new Rect(55, 46, 26, 23), new GUIContent("<", "Go to Previous Layer"), buttonsStyle))
            {
                selectedLayer--;

                currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

                Undo.RecordObject(this, "Recording Selected Level Choice");
                Undo.RecordObject(myTarget, "Recording Selected Level Choice");



                CleanLayer();
                SpawnLayer();
            }
        }
        else
        {
            //Blocage au PREMIER layer
            if (GUI.Button(new Rect(55, 46, 26, 23), new GUIContent("-", "This is the First Layer"), noneStyle))
            {
                //Something
            }
        }

        if (selectedLayer < totalLayersDisplayed)
        {
            //Passage au layer SUIVANT
            if (GUI.Button(new Rect(126, 46, 26, 23), new GUIContent(">", "Go to Next Layer"), buttonsStyle))
            {
                selectedLayer++;

                currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

                CleanLayer();
                SpawnLayer();

                Undo.RecordObject(this, "Recording Selected Level Choice");
                Undo.RecordObject(myTarget, "Recording Selected Level Choice");
            }
        }
        else
        {
            //Incrémentation du nombre TOTAL de layer
            if (GUI.Button(new Rect(130, 49, 20, 18), new GUIContent("+", "Add a Layer"), buttonsStyle))
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

                CleanLayer();
                SpawnLayer();

                Undo.RecordObject(this, "Recording Selected Level Choice");
                Undo.RecordObject(myTarget, "Recording Selected Level Choice");
            }
        }


        if (GUI.Button(new Rect(160, 49, 90, 18), new GUIContent("Reset Layer", "Clean Layer's Data")))
        {
            ResetLayer();

            Undo.RecordObject(this, "Recording Selected Level Choice");
            Undo.RecordObject(myTarget, "Recording Selected Level Choice");
        }

        GUI.Label(new Rect(5, 46, 60, 25), "Layer", layerStyle);


        //Changement du LAYER SELECTIONNE
        EditorGUI.BeginChangeCheck();

        selectedLayer = EditorGUI.IntField(new Rect(74, 49, 22, 18), selectedLayer, layerStyle);

        if (EditorGUI.EndChangeCheck())
        {
            if (selectedLayer > totalLayersDisplayed)
            {
                selectedLayer = totalLayersDisplayed;
            }

            currentLayer = myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer];

            Undo.RecordObject(this, "Recording Selected Level Choice");
            Undo.RecordObject(myTarget, "Recording Selected Level Choice");
        }

        GUI.Label(new Rect(87, 46, 30, 25), "/", slashStyle);



        //Changement du nombre TOTAL de layer
        EditorGUI.BeginChangeCheck();

        totalLayersDisplayed = EditorGUI.IntField(new Rect(107, 49, 22, 18), totalLayersDisplayed, layerStyle);

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

            Undo.RecordObject(this, "Recording Selected Level Choice");
            Undo.RecordObject(myTarget, "Recording Selected Level Choice");
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




    //private void UpdateCurrentPieceInstance(InfoLevelPiece item, Texture2D preview)
    //{
    //    itemSelected = item;
    //    itemPreview = preview;
    //    pieceSelected = (LevelPiece)item.GetComponent<LevelPiece>();
    //    Repaint();
    //}



    private void ModeHandler()
    {
        switch (selectedMode)
        {
            case EditionMode.Paint2D:
            case EditionMode.Erase2D:
                Tools.current = Tool.None;
                break;
            case EditionMode.View:
                Tools.current = Tool.View;
                break;
            case EditionMode.Select:
                Tools.current = Tool.Custom;
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

                newPos2.x = 0f;
                newPos2.y = 0f;
                newPos2.z = 0f;
                newPos2.w = 0f;

                _scene2.rotation = newPos2;
                _scene2.Repaint();

                if ((Event.current.type == EventType.MouseDown || Event.current.type == EventType.MouseDrag) && Event.current.button == 0)
                {
                    Erase(col, row);
                }
                break;

            case EditionMode.Select:
                SceneView _scene3 = SceneView.lastActiveSceneView;
                _scene3.orthographic = true;

                Quaternion newPos3 = _scene3.rotation;

                newPos3.x = 0f;
                newPos3.y = 0f;
                newPos3.z = 0f;
                newPos3.w = 0f;

                _scene3.rotation = newPos3;
                _scene3.Repaint();
                break;

            case EditionMode.View:
                SceneView _scene4 = SceneView.lastActiveSceneView;
                _scene4.orthographic = true;

                Quaternion newPos4 = _scene4.rotation;

                newPos4.x = 0f;
                newPos4.y = 0f;
                newPos4.z = 0f;
                newPos4.w = 0f;

                _scene4.rotation = newPos4;
                _scene4.Repaint();
                break;
        }
    }


    private void Paint(int col, int row)
    {

        if (!myTarget.IsInsideGridBounds(col, row) || prefabBase == null)
        {
            return;
        }

        //Détermine l'INDEX de la brick
        int selectedBrick = col * myTarget.TotalRows + row;

        if (!currentLayer.wallBricks[selectedBrick].isBrickHere)
        {
            //Debug.LogFormat("GridPos {0},{1}", col, row);


            //SPAWN et récupération du BEHAVIOUR
            //GameObject obj = PrefabUtility.InstantiatePrefab(prefabBase) as GameObject;
            GameObject obj = Instantiate(prefabBase) as GameObject;
            BrickBehaviours objBehaviours = obj.GetComponent<BrickBehaviours>();
            MeshRenderer objMesh = obj.GetComponent<MeshRenderer>();
            Material[] mats = objMesh.sharedMaterials;

            mats[1] = new Material(Shader.Find("Shader Graphs/Sh_CubeEdges00"));
            mats[1].SetFloat("_Metallic", 0.75f);

            mats[0] = new Material(Shader.Find("Shader Graphs/Sh_CubeCore01"));
            mats[0].SetColor("_FresnelColor", myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].fresnelColors);
            mats[0].SetColor("_CoreEmissiveColor", myTarget.ColorPreset[0].colorPresets[paintedBrickSettings.brickColorPreset].coreEmissiveColors);
            mats[0].SetFloat("_XFrameThickness", 0.75f);
            mats[0].SetFloat("_YFrameThickness", 0.75f);

            objMesh.sharedMaterials = mats;


            //objMesh.materials[0] = mats[0];

            /*
            if (Shader.Find("Sh_CubeEdges00") != null)
            {
                Debug.Log("Found it");
                
            }
            */



            //POSITION de la brick
            obj.transform.parent = myTarget.transform;

            obj.name = string.Format("{0},{1},{2}", col, row, obj.name);

            obj.transform.position = myTarget.GridToWorldPoint(col, row);


            objBehaviours.armorPoints = paintedBrickSettings.armorValue;
            objBehaviours.scoreValue = paintedBrickSettings.scoreValue;

            if (currentLayer.wallBricks[selectedBrick].isMoving)
            {
                objBehaviours.isMoving = paintedBrickSettings.isMoving;
                objBehaviours.speed = paintedBrickSettings.speed;
                objBehaviours.smoothTime = paintedBrickSettings.smoothTime;
                objBehaviours.waypoints = new Vector3[paintedBrickSettings.waypointsStorage.Length];

                for (int j = 0; j < currentLayer.wallBricks[selectedBrick].waypointsStorage.Length; j++)
                {
                    objBehaviours.waypoints[j] = paintedBrickSettings.waypointsStorage[j];
                }
            }



            //SET des datas
            BrickSettings brick = new BrickSettings();
            brick = paintedBrickSettings;
            brick.brickID = prefabBase.name;
            brick.brickPosition = myTarget.GridToWorldPoint(col, row);
            brick.isBrickHere = true;


            //ENREGISTREMENT des datas
            currentLayer.wallBricks[selectedBrick] = brick;
            myTarget.selectedLevel.level.levelWallBuilds.walls[selectedLayer] = currentLayer;




            myTarget.bricksOnLayer++;
            //RECUPERATION du gameobject crée
            myTarget.bricksOnScreen[selectedBrick] = obj;

            
            Undo.RecordObject(this, "Recording Selected Level Choice");
            Undo.RecordObject(myTarget, "Recording Selected Level Choice");
        }

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


        if (currentLayer.wallBricks[col * myTarget.TotalRows + row].isBrickHere)
        {
            DestroyImmediate(myTarget.bricksOnScreen[col * myTarget.TotalRows + row]);

            BrickSettings blankBrick = new BrickSettings();
            currentLayer.wallBricks[col * myTarget.TotalRows + row] = blankBrick;
        }
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
