using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public enum PlayerID 
{
    NONE = -1,
    PLAYER1 = 0,
    PLAYER2 = 1
}

public enum PlayerHand
{
    RIGHT,
    LEFT
}

public class QPlayerManager : MonoBehaviour
{
    #region Singleton
    public static QPlayerManager instance;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
    }
    #endregion

    public GameObject player1;
    public GameObject player2 = null;

    private GameObject player1RightController;
    private GameObject player1LeftController;

    private GameObject player2RightController;
    private GameObject player2LeftController;

    private void Start()
    {
        StartCoroutine(SetupControllers());
    }


    public GameObject GetController(PlayerID playerID, PlayerHand playerHand)         //A editer pour chaque cas player
    {
        if(playerID == PlayerID.PLAYER1)
        {
            if (playerHand == PlayerHand.RIGHT)
                return player1RightController;
            else
                return player1LeftController;
        }
        else if(playerID == PlayerID.PLAYER2)
        {
            if (playerHand == PlayerHand.RIGHT)
                return player2RightController;
            else
                return player2LeftController;
        }
        
        return  player1LeftController;
    }

    public void OnPlayer1RightTriggerPress()
    {
        ActionCall(PlayerID.PLAYER1, PlayerHand.RIGHT);
    }

    public void OnPlayer1LeftTriggerPress()
    {
        ActionCall(PlayerID.PLAYER1, PlayerHand.LEFT);
    }

    public void OnPlayer2RightTriggerPress()
    {
        ActionCall(PlayerID.PLAYER2, PlayerHand.RIGHT);
    }

    public void OnPlayer2LeftTriggerPress()
    {
        ActionCall(PlayerID.PLAYER2, PlayerHand.LEFT);
    }

    private IEnumerator SetupControllers()
    {
        yield return new WaitForFixedUpdate();
        if(player1)
        {
            player1LeftController = player1?.GetComponentInChildren<LeftControllerGetter>().Get();
            player1RightController = player1?.GetComponentInChildren<RightControllerGetter>().Get();
        }
        if (player2)
        {
            player2LeftController = player2?.GetComponentInChildren<LeftControllerGetter>().Get();
            player2RightController = player2?.GetComponentInChildren<RightControllerGetter>().Get();
        }
    }

    private void ActionCall(PlayerID playerID, PlayerHand playerHand)
    {
        if(RacketManager.instance.GetGrabStatus() == GrabState.GRABBED)
        {
            if(RacketManager.instance.GetRacketUserInfo().userID == playerID)
            {
                if (RacketManager.instance.GetRacketUserInfo().userHand == playerHand)
                    RacketManager.instance.StopRacketGrabCall(playerID);
                else
                    return; //Action ball?
            }
            else
            {
                return; //A enlever?
            }
        }
        else if(RacketManager.instance.GetGrabStatus() == GrabState.ATTRACTED)
        {
            if(RacketManager.instance.GetRacketUserInfo().userID == playerID)
                RacketManager.instance.StopRacketGrabCall(playerID);
        }
        else
        {
            RacketManager.instance.RacketGrabCall(playerID, playerHand);
        }
    }
}
