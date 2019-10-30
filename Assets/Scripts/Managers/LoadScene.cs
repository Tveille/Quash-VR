using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{
    [SerializeField] private string sceneName;

    public void LoadingScene ()
    {
        SceneManager.LoadScene(sceneName);
    }
}
