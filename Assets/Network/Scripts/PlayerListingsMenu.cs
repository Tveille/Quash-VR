using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListingsMenu : MonoBehaviourPunCallbacks
{
     [SerializeField]
    private Transform content;
    [SerializeField]
    private PlayerListing _playerListing;

    private List<PlayerListing> _listings = new List<PlayerListing>();
    private RoomCanvasGroup _roomsCanvases;

    private void Awake(){
        GetCurrentRoomPlayers();
    }

    public void Update()
    {
        Debug.Log(_listings.Count);
    }

    public void FirstInitialize(RoomCanvasGroup canvases){
        _roomsCanvases = canvases;
    }

    public override void OnLeftRoom(){
        content.DestroyChildren();
    }

    private void GetCurrentRoomPlayers(){
        if(!PhotonNetwork.IsConnected)
            return;

        if (PhotonNetwork.CurrentRoom == null || PhotonNetwork.CurrentRoom.Players == null)
            return;

        foreach(KeyValuePair<int, Player> playerInfo in PhotonNetwork.CurrentRoom.Players){
            AddPlayerListing(playerInfo.Value);
        }        
    }

    private void AddPlayerListing(Player player){
        PlayerListing listing = Instantiate(_playerListing, content);
        
            if (listing != null){
                listing.SetPlayerInfo(player);
                _listings.Add(listing);
            }

        Debug.Log(listing);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer){
       AddPlayerListing(newPlayer);
       Debug.Log(_listings);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        int index = _listings.FindIndex(x => x.Player == otherPlayer);
        if (index != -1){
            Destroy(_listings[index].gameObject);
            _listings.RemoveAt(index);
        }
    }

    public void OnClick_StartGame(){
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.CurrentRoom.IsVisible= false;
            PhotonNetwork.LoadLevel(1);
        }
    }
}

