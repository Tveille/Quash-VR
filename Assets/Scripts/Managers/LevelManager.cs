using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public LevelsScriptable[] registeredLevels;
    public LevelSettings currentLevelConfig;


    [Header("Level Parameters")]
    public int currentLayer = -1;
    public bool isThereAnotherLayer;

    public float layerDiffPosition;

    public Transform levelTrans;
    public Vector3 startPos;
    public Vector3 NextPos;

    public Vector3 refVector;
    [Range(0, 1)] public float smoothTime;
    [Range(0.1f, 10)] public float sMaxSpeed;

    public bool changePositionReady = false;


    public static LevelManager Instance;



    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (changePositionReady)
        {
            NextLayer();
        }
    }



    /// <summary>
    /// Go to the next Layer
    /// </summary>
    void NextLayer()
    {
        levelTrans.position = Vector3.SmoothDamp(levelTrans.position, NextPos, ref refVector, smoothTime, sMaxSpeed);
    }



    /// <summary>
    /// Set up parameters to change level position
    /// </summary>
    public void SetNextLayer()
    {
        currentLayer += 1;
        NextPos = new Vector3(0, 0, startPos.z + (layerDiffPosition * currentLayer));

        if (currentLayer >= currentLevelConfig.levelWallBuilds.walls.Length)
        {
            isThereAnotherLayer = false;
        }

        changePositionReady = true;
    }



    /// <summary>
    /// Attribute data to corresponding managers
    /// </summary>
    /// <param name="selectedLevel"></param>
    public void ConfigDistribution(int selectedLevel)
    {
        currentLevelConfig = registeredLevels[selectedLevel].level;
        BrickManager.Instance.levelWallsConfig = currentLevelConfig.levelWallBuilds;
        ZoneManager.Instance.levelZoneConfig = currentLevelConfig.levelZoneConfig;
    }
}
