using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{
    public static event Action<GameState> OnGameStateChanged;
    public GameState State = GameState.START;
    public PlayerManager PlayerManager;
    public UIManager UIManager;

    public int TurnOrder = 0;
    public int PlayerHealth = 50;
    public int EnemyHealth = 50;

    private int ReadyClicks = 0;

    public bool once = true;
    private void Start()
    {
        UpdateGameState(GameState.START);
        UIManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        UIManager.UpdatePlayerHealth();
    }

    public void UpdateGameState(GameState newState)
    {
        
        switch (newState)
        {
            case GameState.START:
                ReadyClicks = 0;
                State = GameState.START;
                break;
            case GameState.İNGAME:
                if (ReadyClicks == 2)
                {                    
                    if(hasAuthority)
                    {
                        State = GameState.YOURTURN;
                    }
                    else
                    {
                        State = GameState.OPPONENTTURN;
                    }
                }
            case GameState.WİN:
                State = GameState.WİN;
                break;
            case GameState.LOST:
                State = GameState.LOST;
                break;

        }
        OnGameStateChanged?.Invoke(newState);
        Debug.Log(State);
    }
    public void ChangeReadyClicks()
    {
        ReadyClicks++;
    }
    public void CardPlayed()
    {
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();
        TurnOrder++;
        if (TurnOrder == 2)
        {
            PlayerManager pm = NetworkClient.connection.identity.GetComponent<PlayerManager>();
            pm.isMyTurn = !pm.isMyTurn;
            TurnOrder = 0;
        }
    }
    public void ChangeHealth(int PlayerHitPoints, int OpponentHitPoints, bool hasAuthority)
    {
        if (hasAuthority)
        {
            PlayerHealth += PlayerHitPoints;
            EnemyHealth -= OpponentHitPoints; 
        }
        else
        {
            PlayerHealth -= OpponentHitPoints;
            EnemyHealth += PlayerHitPoints;
        }
        UIManager.UpdatePlayerHealth();
    }
}
public enum GameState
{
    START,
    İNGAME,
    YOURTURN,
    OPPONENTTURN,
    WİN,
    LOST
}