using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;
using TMPro;


public class CreateRoom : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private TextMeshProUGUI _roomName;
    private RoomCanvasGroup roomCanvases;
    
    public Button buttonCreate;

    void Awake(){
 
    }
    public void FirstInitialize(RoomCanvasGroup canvases){
        roomCanvases = canvases;
    }

    public void OnClick_CreateRoom(){
        if(!PhotonNetwork.IsConnected){
            return;
        }
            

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 2;
        PhotonNetwork.JoinOrCreateRoom(_roomName.text, options, TypedLobby.Default);
    }

    public override void OnCreatedRoom(){
      Debug.Log("Created room successfully.");
      roomCanvases.CurrentRoomCanvas.Show();
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        Debug.Log("Room creation failed:" + message, this);
    }
}
