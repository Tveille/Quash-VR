using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
    // public static LevelScript Instance;

    [SerializeField]
    public string nameLevel;

    private int totalRows = 10; //x

    private int totalColumns = 10; //y

    public float CellSize = 0.5f;
    private readonly Color normalColor = Color.red;
    private readonly Color selectedColor = Color.green;

    public Vector3[] editorSpace = new Vector3[2];

    public float xGridPlacement = 2f;

    public float yGridPlacement = 1f;

    public float zGridPlacement;


    [SerializeField]
    public Sprite background;


    
    public LevelsScriptable[] allLevels = new LevelsScriptable[0];

    public LevelsScriptable selectedLevel;
    public LevelSettings levelCategories;

    public int numberOfWallLayer;
    public int currentWallLayer;






    public int TotalRows
    {
        get { return totalRows; }
        set { totalRows = value; }
    }

    public int TotalColumns
    {
        get { return totalColumns; }
        set { totalColumns = value; }
    }



    public void FromLevelScriptToLevelManager()
    {
        //LevelManager.Instance.registeredLevels = new LevelsScriptable[10];
        //LevelManager.Instance.registeredLevels = allLevels;
    }

    public float maxWidthSpace()
    {
        return editorSpace[1].x - editorSpace[0].x;
    }

    private float colSpace()
    {
        return CellSize * (float)totalColumns;
    }

    public float maxHeightSpace()
    {
        return editorSpace[1].y - editorSpace[0].y;
    }

    private float rowSpace()
    {
        return CellSize * (float)totalRows;
    }

    private float WidthOffset()
    {
        return (maxWidthSpace() - colSpace()) / 2;
    }



    private void GridFrameGizmo(int cols, int rows)
    {
        /* LEFT */
        Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            yGridPlacement,
            zGridPlacement),

            new Vector3(xGridPlacement + WidthOffset(),
            (rows * CellSize) + yGridPlacement,
            zGridPlacement));

        ///* RIGHT */
        Gizmos.DrawLine(new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
                    yGridPlacement,
                    zGridPlacement),

                    new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
                    (rows * CellSize) + yGridPlacement,
                    zGridPlacement));

        ///* BOTTOM */
        Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            yGridPlacement,
            zGridPlacement),

            new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
            yGridPlacement,
            zGridPlacement));



        ///* UP */
        Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            (rows * CellSize) + yGridPlacement,
            zGridPlacement),

            new Vector3((cols * CellSize) + xGridPlacement + WidthOffset(),
            (rows * CellSize) + yGridPlacement,
            zGridPlacement));
    }

    private void GridGizmo(int cols, int rows)
    {
        for (int i = 1; i < cols; i++)
        {
            //COLUMNS
            Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset() + (i * CellSize),
                yGridPlacement,
                zGridPlacement),

                new Vector3(xGridPlacement + WidthOffset() + (i * CellSize),
                (rows * CellSize) + yGridPlacement,
                zGridPlacement));
        }


        for (int j = 1; j < rows; j++)
        {
            //ROWS
            Gizmos.DrawLine(new Vector3(xGridPlacement + WidthOffset(),
            ((j * CellSize) + yGridPlacement),
            zGridPlacement),

            new Vector3((xGridPlacement + WidthOffset() + (cols * CellSize)),
            ((j * CellSize) + yGridPlacement),
            zGridPlacement));
        }
    }

    private void OnDrawGizmos()
    {
        Color oldColor = Gizmos.color;
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;

        Gizmos.color = normalColor;
        GridGizmo(TotalColumns, TotalRows);
        GridFrameGizmo(TotalColumns, TotalRows);

        Gizmos.color = oldColor;
        Gizmos.matrix = oldMatrix;
    }



    /// <summary>
    /// Receive a Vector3 and return a vector 3 where x and y correspond to cols and rows coordinates
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 WorldToGridCoordinates(Vector3 point)
    {
        Vector3 gridPoint = new Vector3(
            (int)((point.x - xGridPlacement - WidthOffset()) / CellSize),
             (int)((point.y - yGridPlacement) / CellSize),
             zGridPlacement);

        return gridPoint;
    }

    /// <summary>
    /// Receive col and row position and return Vector3 corresponding to world coordinates(y = 0)
    /// </summary>
    /// <param name="col"></param>
    /// <param name="row"></param>
    /// <returns></returns>
    public Vector3 GridToWorldPoint(int col, int row)
    {
        Vector3 worldPoint = new Vector3(
            xGridPlacement + WidthOffset() + (col * CellSize) + (CellSize/2),
            yGridPlacement + (row * CellSize) + (CellSize/2),
            zGridPlacement);
        return worldPoint;
    }



    /// <summary>
    /// Booleans receive a vector 3 and return true or false if coordinates are inside the grid
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public bool IsInsideGridBounds(Vector3 point)
    {
        float minX = xGridPlacement - WidthOffset();
        float maxX = (TotalColumns * CellSize) - minX;
        Debug.Log("min : " + minX + " max : " + maxX);

        float minY = yGridPlacement;
        float maxY = minY + TotalRows * CellSize;

        return (point.x >= minX && point.x <= maxX && point.y >= minY && point.y <= maxY);
    }

    public bool IsInsideGridBounds(int col, int row)
    {
        return (col >= 0 && col < TotalColumns && row >= 0 && row < TotalRows);
    }
}
