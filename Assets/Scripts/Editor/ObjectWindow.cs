using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectWindow : EditorWindow
{
    private List<InfoLevelPiece.Category> categories;
    private List<string> categoryLabel;
    private InfoLevelPiece.Category categorySelected;


    public static ObjectWindow instance;
    private string path = "Assets/Prefabs/Bricks";
    private List<InfoLevelPiece> items;
    private Dictionary<InfoLevelPiece.Category, List<InfoLevelPiece>> categorizedItems;
    private Dictionary<InfoLevelPiece, Texture2D> previews;
    private Vector2 scrollPosition;
    private const float buttonWidth = 80;
    private const float buttonHeight = 90;
    private GUIStyle tabStyle;


    public delegate void itemSelectedDelegate(InfoLevelPiece item, Texture2D preview); //Receive an item and a texture
    public static event itemSelectedDelegate ItemSelectedEvent;



    private void InitStyles()
    {
        tabStyle = new GUIStyle();
        tabStyle.alignment = TextAnchor.MiddleCenter;
        tabStyle.fontSize = 16;

        tabStyle.normal.textColor = new Color(0.145f, 0.58f, .255f, 1f);
    }

    public static void ShowWindow()
    {
        instance = (ObjectWindow)EditorWindow.GetWindow(typeof(ObjectWindow));
        instance.titleContent = new GUIContent("Object Window");
    }

    private void OnEnable()
    {
        if (categories == null)
        {
            InitCategories();
        }
        if (categorizedItems == null)
        {
            InitContent();
        }
    }

    private void InitContent()
    {
        items = EditorUtilityScene.GetAssetsWithScript<InfoLevelPiece>(path);
        categorizedItems = new Dictionary<InfoLevelPiece.Category, List<InfoLevelPiece>>();
        previews = new Dictionary<InfoLevelPiece, Texture2D>();

        foreach (InfoLevelPiece.Category category in categories)
        {
            categorizedItems.Add(category, new List<InfoLevelPiece>());
        }

        foreach (InfoLevelPiece item in items)
        {
            categorizedItems[item.category].Add(item);
        }
    }

    private void InitCategories()
    {
        categories = EditorUtilityScene.GetListFromEnum<InfoLevelPiece.Category>();
        categoryLabel = new List<string>();

        foreach (InfoLevelPiece.Category category in categories)
        {
            categoryLabel.Add(category.ToString());
        }
    }

    private void DrawTabs()
    {
        int index = (int)categorySelected;
        index = GUILayout.Toolbar(index, categoryLabel.ToArray(), tabStyle);

        categorySelected = categories[index];
    }

    /// <summary>
    /// Draw les items
    /// </summary>
    private void DrawScroll()
    {
        if (categorizedItems[categorySelected].Count == 0)
        {
            EditorGUILayout.HelpBox("La catégorie est vide ! ", MessageType.Info);
            return;
        }

        int rowCapacity = Mathf.FloorToInt(position.width / (buttonWidth));

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        int selectionGridIndex = -1;
        selectionGridIndex = GUILayout.SelectionGrid(selectionGridIndex, GetGUIContentsFromItems(), rowCapacity, GetGUIStyle());

        GetSelectedItem(selectionGridIndex);

        GUILayout.EndScrollView();
    }


    /// <summary>
    /// Génère une Dictionary de key "InfoLevelPiece" et d'élément Texture2D pour l'affichage en window
    /// </summary>
    private void GeneratePreviews()
    {
        foreach (InfoLevelPiece item in items)
        {
            if (!previews.ContainsKey(item))
            {
                Texture2D preview = AssetPreview.GetAssetPreview(item.gameObject);


                if (preview != null)
                {
                    previews.Add(item, preview);
                }
            }
        }
    }


    private GUIContent[] GetGUIContentsFromItems()
    {
        List<GUIContent> guiContents = new List<GUIContent>();

        if (previews.Count == items.Count)
        {
            int totalItems = categorizedItems[categorySelected].Count;

            for (int i = 0; i < totalItems; i++)
            {
                GUIContent guiContent = new GUIContent();
                guiContent.text = categorizedItems[categorySelected][i].itemName;
                guiContent.image = previews[categorizedItems[categorySelected][i]];
                guiContents.Add(guiContent);
            }
        }

        return guiContents.ToArray();
    }

    private GUIStyle GetGUIStyle()
    {
        GUIStyle guiStyle = new GUIStyle(GUI.skin.button);

        guiStyle.alignment = TextAnchor.LowerCenter;
        guiStyle.imagePosition = ImagePosition.ImageAbove;
        guiStyle.fixedWidth = buttonWidth;
        guiStyle.fixedHeight = buttonHeight;

        return guiStyle;
    }

    private void GetSelectedItem(int index)
    {
        if (index != -1)
        {
            InfoLevelPiece selectedItem = categorizedItems[categorySelected][index];

            if (ItemSelectedEvent != null)
            {
                ItemSelectedEvent(selectedItem, previews[selectedItem]);
            }
        }
    }


    private void Update()
    {
        if (previews.Count != items.Count)
        {
            GeneratePreviews();
        }
    }


    private void OnDisable()
    {

    }

    private void OnDestroy()
    {

    }


    private void OnGUI()
    {
        DrawTabs();
        DrawScroll();
    }

}
