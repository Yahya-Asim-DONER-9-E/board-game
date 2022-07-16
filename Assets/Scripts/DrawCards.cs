using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DrawCards : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public UIManager UIManager;

    

    private void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }
    public void OnClick()
    {
        
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();          
       
        if (GameManager.State == GameState.START && GameManager.once)
        {
            StartClick();
            GameManager.once = false;
        }
        else if (GameManager.State == GameState.YOURTURN)
        {
            InGameClick();
        }    
    }
    void StartClick()
    {
        PlayerManager.CmdDealCards();
        PlayerManager.CmdSetAvailable();
        PlayerManager.CmdGMChangeState(GameState.İNGAME);
    }
    void InGameClick()
    {
        PlayerManager.CmdCheckPoints();
        PlayerManager.CmdSetAvailable();
        if (PlayerManager.PlayerArea.transform.childCount + PlayerManager.EnemyArea.transform.childCount == 0)
        {
            PlayerManager.CmdGMChangeState(GameState.START);
            PlayerManager.CmdChangeOnceToTrue();
        }
        if(GameManager.PlayerHealth == 0)
        {
            PlayerManager.CmdGMChangeState(GameState.LOST);
        }
        else if(GameManager.EnemyHealth == 0)
        {
            PlayerManager.CmdGMChangeState(GameState.WİN);
        }
    }
}
