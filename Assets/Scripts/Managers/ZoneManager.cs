using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("Récupération de la configuration du level")]
    public LayerZoneConfigurations levelZoneConfig;


    public static ZoneManager Instance;





    private void Awake()
    {
        Instance = this;
    }
}
