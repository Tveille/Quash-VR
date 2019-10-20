using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{

    public GameObject prefabPlayer;
    public GameObject menu;

    void Awake(){
        PhotonNetwork.Instantiate(prefabPlayer.name, prefabPlayer.transform.position, Quaternion.identity, 0);
    }

    public void RestartScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Menu()
    {
        menu.SetActive(!menu.activeSelf);
    }
}
