﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScript : MonoBehaviour
{
  // public static LevelScript Instance;
   
   [SerializeField]
   public string nameLevel;
   [SerializeField]
   private int totalColumns = 25;
   [SerializeField]
   private int totalRows = 10;
   public const float GridSize = 1.28f;
   private readonly Color normalColor = Color.red;
   private readonly Color selectedColor = Color.yellow;
   [SerializeField]
   private LevelPiece[] pieces;
   [SerializeField]
   public Sprite background;
   [SerializeField]
   private LevelSettings settings;
   Rigidbody rb;

   void Awake(){
     rb = GetComponent<Rigidbody>();
   }

    public LevelSettings Settings{
        get { return settings; }
        set { settings = value; }
    }
   public LevelPiece[] Pieces{
       get { return pieces; }
       set { pieces = value; }
   }

   public int TotalColumns{
       get { return totalColumns; }
       set { totalColumns = value; }
   }

   public int TotalRows{
       get { return totalRows; }
       set { totalRows = value; }
   }

   private void GridFrameGizmo(int cols, int rows){
       Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(0, 0, cols * GridSize));
       Gizmos.DrawLine(new Vector3(0, 0, 0), new Vector3(rows * GridSize, 0, 0));
       Gizmos.DrawLine(new Vector3(rows * GridSize, 0, 0), new Vector3(rows * GridSize, 0, cols * GridSize));
       Gizmos.DrawLine(new Vector3(0, 0, cols * GridSize), new Vector3(rows * GridSize, 0, cols * GridSize));
   }

   private void GridGizmo(int cols, int rows){
       for (int i =1; i < rows; i++){
           Gizmos.DrawLine(new Vector3(i * GridSize, 0, 0), new Vector3(i * GridSize, 0, cols * GridSize));
       }
       for (int j = 1; j < cols; j++){
           Gizmos.DrawLine(new Vector3(0, 0, j * GridSize), new Vector3(rows * GridSize, 0, j * GridSize));
       }
   }



   private void OnDrawGizmos(){
       Color oldColor = Gizmos.color;
       Matrix4x4 oldMatrix = Gizmos.matrix;
       Gizmos.matrix = transform.localToWorldMatrix;

       Gizmos.color = normalColor;
       GridGizmo(totalColumns, totalRows);
       GridFrameGizmo(TotalColumns, totalRows);

       Gizmos.color = oldColor;
       Gizmos.matrix = oldMatrix;
   }

    //Receive a Vector3 and return a vector 3 where x and z correspond to cols and rows coordinates
   public Vector3 WorldToGridCoordinates(Vector3 point){
       Vector3 gridPoint = new Vector3(
           (int)((point.x - transform.position.x)/GridSize),
            0.0f,(int)((point.z - transform.position.z)/GridSize));
       return gridPoint;
   }

    //Receive col and row position and return Vector3 corresponding to world coordinates(y = 0)
   public Vector3 GridToWorldPoint(int col, int row){
       Vector3 worldPoint =  new Vector3(
           transform.position.x + (col * GridSize + GridSize / 2.0f),
           0.0f,
           transform.position.z + (row * GridSize + GridSize / 2.0f));
        return worldPoint;
   }

    //Booleans receive a vector 3 and return true or false if coordinates are inside the grid
   public bool IsInsideGridBounds(Vector3 point){
       float minX = transform.position.x;
       float maxX = minX + totalColumns * GridSize;
       float minZ = transform.position.z;
       float maxZ = minZ + totalRows * GridSize;
       return(point.x >= minX && point.x <= maxX && point.z >= minZ && point.z <= maxZ);
   }

   public bool IsInsideGridBounds(int col, int row){
       return(col >= 0 && col <totalColumns && row >= 0 && row < totalRows);
   }

   

}