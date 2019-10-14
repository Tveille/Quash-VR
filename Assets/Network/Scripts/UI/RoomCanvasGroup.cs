using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomCanvasGroup : MonoBehaviour
{
   [SerializeField]
   private CreateOrJoinRoomCanvas createOrJoinCanvas;
   public CreateOrJoinRoomCanvas CreateOrJoinRoomCanvas { get { return createOrJoinCanvas;} }

   [SerializeField]
   private CurrentRoomCanvas currentRoomCanvas;
   public CurrentRoomCanvas CurrentRoomCanvas { get { return currentRoomCanvas; } }

    private void Awake() {
        FirstInitialize();
    }

    private void FirstInitialize(){
        CreateOrJoinRoomCanvas.FirstInitialize(this);
        CurrentRoomCanvas.FirstInitialize(this);
        
    }
}
