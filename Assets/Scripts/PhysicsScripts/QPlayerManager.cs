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
    //public GameObject player2 = null;

    private GameObject player1RightController;
    private GameObject player1LeftController;

    //private GameObject player2RightController;
    //private GameObject player2LeftController;

    private void Start()
    {
        StartCoroutine(SetupControllers());
    }


    public GameObject GetRightController(PlayerID playerID)         //A editer pour chaque cas player
    {
        return player1RightController;
    }

    public GameObject GetLeftController(PlayerID playerID)          //A editer pour chaque cas player
    {
        return player1LeftController;
    }

    public void OnPlayer1RightTriggerPress()
    {
        RacketManager.instance.RacketGrabCall(PlayerID.PLAYER1);
    }

    public void OnPlayer1RightTriggerRelease()
    {
        RacketManager.instance.StopRacketGrabCall(PlayerID.PLAYER1);
    }

    private IEnumerator SetupControllers()
    {
        yield return new WaitForFixedUpdate();
        player1LeftController = player1?.GetComponentInChildren<LeftControllerGetter>().Get();
        //player2LeftController = player2?.GetComponentInChildren<LeftControllerGetter>().Get();
        player1RightController = player1?.GetComponentInChildren<RightControllerGetter>().Get();
        //player2RightController = player2?.GetComponentInChildren<RightControllerGetter>().Get();
    }
}
