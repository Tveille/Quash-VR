using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestConnect : MonoBehaviourPunCallbacks
{
   private void Start(){
       Screen.fullScreen = !Screen.fullScreen;

       Debug.Log("Connecting to Photon...", this);
       PhotonNetwork.SendRate = 40;
       PhotonNetwork.SerializationRate = 40;
       PhotonNetwork.AutomaticallySyncScene = true;
       PhotonNetwork.NickName = MasterManager.GameSettings.NickName;
       PhotonNetwork.GameVersion = MasterManager.GameSettings.GameVersion;
       PhotonNetwork.ConnectUsingSettings();
   }

   public override void OnConnectedToMaster(){
       Debug.Log("Connected to Photon", this);
       Debug.Log(PhotonNetwork.LocalPlayer.NickName, this);

       if (!PhotonNetwork.InLobby){
           PhotonNetwork.JoinLobby();
       }  
   }

   public override void OnDisconnected(Photon.Realtime.DisconnectCause cause){
       Debug.Log("Disconnected from server for reason " + cause.ToString(), this);
   }

   public override void OnJoinedLobby(){
       print("Joined lobby");
   }
}
