using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateOrJoinRoomCanvas : MonoBehaviour
{

    [SerializeField] 
    private CreateRoom createRoomMenu;
    [SerializeField] 
    private RoomListingsMenu _roomListingsMenu;

    private RoomCanvasGroup roomCanvases;
    public void FirstInitialize(RoomCanvasGroup canvases){
        roomCanvases = canvases;
        createRoomMenu.FirstInitialize(canvases);
        _roomListingsMenu.FirstInitialize(canvases);
    }
}
