using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolLevels : MonoBehaviour
{

    public GameObject[] levels;

    
    // Start is called before the first frame update
    void Start()
    {
        int levelNumber = Random.Range(0, levels.Length);
        Instantiate(levels[levelNumber], levels[levelNumber].transform.position, levels[levelNumber].transform.rotation);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
