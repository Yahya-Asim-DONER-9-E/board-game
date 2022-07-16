using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class UIManager : NetworkBehaviour
{
    public PlayerManager PlayerManager;
    public GameManager GameManager;
    public GameObject Button;
    public GameObject PlayerHealth;
    public GameObject EnemyHealth;
    void Start()
    {
        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void UpdatePlayerHealth()
    {
        PlayerHealth.GetComponentInChildren<Text>().text = GameManager.PlayerHealth.ToString();
        EnemyHealth.GetComponentInChildren<Text>().text = GameManager.EnemyHealth.ToString();
    }
    public void UpdateButtonText(string gameState)
    {
        
        Button.transform.GetChild(0).GetComponent<Text>().text = gameState;
    }

}
