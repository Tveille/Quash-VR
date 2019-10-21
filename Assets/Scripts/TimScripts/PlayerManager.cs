using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerID 
{
    NONE = 0,
    PLAYER1 = 1,
    PLAYER2 = 2
};

public class PlayerManager : MonoBehaviour
{
    public GameObject Player1;
    public GameObject Player2;

    
    public void OnPlayer1RightActionCall()
    {
        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnActionCall(PlayerID.PLAYER1);
    }

    public void OnPlayer1RightStopCall()
    {
        GameObject.Find("RacketManager").GetComponent<RacketManagerScript>().OnStopCall(PlayerID.PLAYER1);
    }
}
