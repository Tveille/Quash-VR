using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class LeaveRoomMenu : MonoBehaviour
{
   private RoomCanvasGroup roomCanvases;

   public void FirstInitialize(RoomCanvasGroup canvases){
       roomCanvases = canvases;
   }

   public void OnClick_LeaveRoom(){
       PhotonNetwork.LeaveRoom(true);
       roomCanvases.CurrentRoomCanvas.Hide();
   }
}
