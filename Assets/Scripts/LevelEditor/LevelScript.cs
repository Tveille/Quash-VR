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
    public float gridWidth;

    public float yGridPlacement = 1f;
    public float gridHeight;

    public float zGridPlacement;


    [SerializeField]
    private LevelPiece[] pieces;

    [SerializeField]
    public Sprite background;

    [SerializeField]
    private LevelSettings settings;







    public LevelSettings Settings
    {
        get { return settings; }
        set { settings = value; }
    }

    public LevelPiece[] Pieces
    {
        get { return pieces; }
        set { pieces = value; }
    }



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


    private void GridFrameGizmo(int cols, int rows)
    {
        /* LEFT */
        Gizmos.DrawLine(new Vector3(xGridPlacement * gridWidth,
            yGridPlacement,
            zGridPlacement),

            new Vector3(xGridPlacement * gridWidth,
            ((cols * CellSize) + yGridPlacement + CellSize) * gridHeight,
            zGridPlacement));

        /* RIGHT */
        Gizmos.DrawLine(new Vector3(((rows * CellSize) + xGridPlacement + CellSize) * gridWidth,
                    yGridPlacement,
                    zGridPlacement),

                    new Vector3(((rows * CellSize) + xGridPlacement + CellSize) * gridWidth,
                    ((cols * CellSize) + yGridPlacement + CellSize) * gridHeight,
                    zGridPlacement));

        /* BOTTOM */
        Gizmos.DrawLine(new Vector3(xGridPlacement * gridWidth,
            yGridPlacement,
            zGridPlacement),

            new Vector3(((rows * CellSize) + CellSize + xGridPlacement) * gridWidth,
            yGridPlacement,
            zGridPlacement));



        /* UP */
        Gizmos.DrawLine(new Vector3(xGridPlacement * gridWidth,
            ((cols * CellSize) + CellSize + yGridPlacement) * gridHeight,
            zGridPlacement),

            new Vector3(((rows * CellSize) + xGridPlacement + CellSize) * gridWidth,
            ((cols * CellSize) + yGridPlacement + CellSize) * gridHeight,
            zGridPlacement));
    }

    private void GridGizmo(int cols, int rows)
    {
        float widthOffset = (maxWidthSpace() - colSpace()) / 2;
        float heightOffset = maxHeightSpace() - rowSpace();


        for (int i = -1; i < cols + 2; i++)
        {
            //COLUMNS
            Gizmos.DrawLine(new Vector3(xGridPlacement + widthOffset + (i * CellSize),
                yGridPlacement,
                zGridPlacement),

                new Vector3(xGridPlacement + widthOffset + (i * CellSize),
                (rows * CellSize) + yGridPlacement,
                zGridPlacement));
        }


        for (int j = 0; j < rows + 1; j++)
        {
            //ROWS
            Gizmos.DrawLine(new Vector3(xGridPlacement + widthOffset - CellSize,
            ((j * CellSize) + yGridPlacement),
            zGridPlacement),

            new Vector3((xGridPlacement + CellSize + widthOffset + (cols * CellSize)),
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
        //GridFrameGizmo(TotalRows, TotalColumns);

        Gizmos.color = oldColor;
        Gizmos.matrix = oldMatrix;
    }



    /// <summary>
    /// Receive a Vector3 and return a vector 3 where x and z correspond to cols and rows coordinates
    /// </summary>
    /// <param name="point"></param>
    /// <returns></returns>
    public Vector3 WorldToGridCoordinates(Vector3 point)
    {
        Vector3 gridPoint = new Vector3(
            (int)((point.x - transform.position.x) / CellSize),
             (int)((point.y - transform.position.z) / CellSize), 
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
            transform.position.x + (col * CellSize + CellSize / 2.0f),
            transform.position.y + (row * CellSize + CellSize / 2.0f),
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
        float minX = transform.position.x;
        float maxX = minX + totalRows * CellSize;
        float minZ = transform.position.z;
        float maxZ = minZ + totalColumns * CellSize;
        return (point.x >= minX && point.x <= maxX && point.z >= minZ && point.z <= maxZ);
    }

    public bool IsInsideGridBounds(int col, int row)
    {
        return (col >= 0 && col < totalRows && row >= 0 && row < totalColumns);
    }
}
