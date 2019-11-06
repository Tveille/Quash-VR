using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VRTK;

public enum PlayerID 
{
    NONE = -1,
    PLAYER1 = 0,
    PLAYER2 = 1
};

public class PlayerManager : MonoBehaviour
{
    #region Singleton
    public static PlayerManager instance;

    private void Awake()
    {
        if(instance == null)
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

    public GameObject Player1;
    public GameObject Player2;


    public void OnPlayer1RightTriggerPress()
    {
        RacketManager.instance.RacketGrabCall(PlayerID.PLAYER1);
    }

    public void OnPlayer1RightTriggerRelease()
    {
        RacketManager.instance.StopRacketGrabCall(PlayerID.PLAYER1);
    }

    public GameObject GetRightController(PlayerID playerID)
    {
        return VRTK_DeviceFinder.GetControllerRightHand(true);
    }

    public Transform GetRigthControllerTransform(PlayerID playerID)
    {
        return VRTK_DeviceFinder.GetControllerRightHand(true).transform;            // Valable que s'il y a un seul player... Refaire la methode à la main pour le multi player
        
    }

    public GameObject GetLeftController(PlayerID playerID)
    {
        return VRTK_DeviceFinder.GetControllerLeftHand(true);
    }

    public Transform GetLeftControllerTransform(PlayerID playerID)
    {
        return VRTK_DeviceFinder.GetControllerLeftHand(true).transform;            // Valable que s'il y a un seul player... Refaire la methode à la main pour le multi player
    }
}
