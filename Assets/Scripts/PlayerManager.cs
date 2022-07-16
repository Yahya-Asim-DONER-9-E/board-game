using Mirror;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class PlayerManager : NetworkBehaviour
{
    public GameManager GameManager;

    public GameObject Fire;
    public GameObject Earth;
    public GameObject Air;
    public GameObject Water;
    public GameObject Sun;
    public GameObject Moon;
    public GameObject Spirit;
    public GameObject Blood;
    public GameObject Lightning;

    public GameObject PlayerArea;
    public GameObject EnemyArea;
    
    public bool isMyTurn = false;

    public GameObject Deck;
    List<GameObject> cardnames = new List<GameObject>();

    public GameObject D00;
    public GameObject D01;
    public GameObject D02;
    public GameObject D10;
    public GameObject D11;
    public GameObject D12;
    public GameObject D20;
    public GameObject D21;
    public GameObject D22;
    List<GameObject> dropZones = new List<GameObject>();

    
    
    public string elmnt = "";
    public bool isFull = false;

    private GameObject C00; private GameObject Cc00;
    private GameObject C01; private GameObject Cc01;
    private GameObject C02; private GameObject Cc02;
    private GameObject C10; private GameObject Cc10;
    private GameObject C11; private GameObject Cc11;
    private GameObject C12; private GameObject Cc12;
    private GameObject C20; private GameObject Cc20;
    private GameObject C21; private GameObject Cc21;
    private GameObject C22; private GameObject Cc22;

    public override void OnStartClient()
    {
        base.OnStartClient();

        GameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        PlayerArea = GameObject.Find("PlayerArea");
        EnemyArea = GameObject.Find("EnemyArea");
        D00 = GameObject.Find("DropZone00");
        D01 = GameObject.Find("DropZone01");
        D02 = GameObject.Find("DropZone02");
        D10 = GameObject.Find("DropZone10");
        D11 = GameObject.Find("DropZone11");
        D12 = GameObject.Find("DropZone12");
        D20 = GameObject.Find("DropZone20");
        D21 = GameObject.Find("DropZone21");
        D22 = GameObject.Find("DropZone22");
        GameObject[] array = new GameObject[9] { D01, D00, D02, D10, D11, D12, D20, D21, D22 };
        dropZones.AddRange(array);
        if (isClientOnly)
        {
            isMyTurn = true;
        }
    }

    [Server]
    public override void OnStartServer()
    {
        base.OnStartServer();
        for (int i = 0; i < 4; i++)
        {
            Deck.GetComponent<Deck>().deck.Add(Fire);
            Deck.GetComponent<Deck>().deck.Add(Water);
            Deck.GetComponent<Deck>().deck.Add(Earth);
            Deck.GetComponent<Deck>().deck.Add(Air);
            Deck.GetComponent<Deck>().deck.Add(Sun);
            Deck.GetComponent<Deck>().deck.Add(Moon);
            Deck.GetComponent<Deck>().deck.Add(Lightning);
            Deck.GetComponent<Deck>().deck.Add(Spirit);
            Deck.GetComponent<Deck>().deck.Add(Blood);
        }
    }
    [Command]
    public void CmdDealCards()
    {

        // if (PlayerArea.transform.childCount > 0 || EnemyArea.transform.childCount > 0) return;
        if (Deck.GetComponent<Deck>().deck.Count >= 4)
        {
            for (int i = 0; i < 4; i++)
            {
                int a = Random.Range(0, Deck.GetComponent<Deck>().deck.Count);
                GameObject card = Instantiate(Deck.GetComponent<Deck>().deck[a], new Vector2(0, 0), Quaternion.identity);
                Deck.GetComponent<Deck>().deck.Remove(Deck.GetComponent<Deck>().deck[a]);
                NetworkServer.Spawn(card, connectionToClient);
                card.transform.SetParent(PlayerArea.transform, false);
                RpcShowCard(card, null, "Dealt");
            }
            RpcGMChangeState(GameState.İNGAME);
        }
        else
        {
            Debug.Log("Deck Sinked");
        }
    }

    public void PlayCard(GameObject card, GameObject dropZone)
    {
        CmdPlayCard(card, dropZone);
    }

    [Command]
    void CmdPlayCard(GameObject card, GameObject dropZone)
    {
        RpcShowCard(card, dropZone, "Played");
    }

    [ClientRpc]
    void RpcShowCard(GameObject card, GameObject dropzone, string type)
    {
        if (type == "Dealt")
        {
            if (hasAuthority)
            {
                card.transform.SetParent(PlayerArea.transform, false);
            }
            else
            {
                card.transform.SetParent(EnemyArea.transform, false);
            }
        }
        else if (type == "Played")
        {
            if (hasAuthority)
            {
                CmdGMCardPlayed();
            }
            card.transform.SetParent(dropzone.transform, false);
            card.gameObject.tag = "Zoomable";
        }
    }
    [Command]
    public void CmdGMChangeState(GameState stateRequest)
    {
        RpcGMChangeState(stateRequest);
    }
    [ClientRpc]
    void RpcGMChangeState(GameState stateRequest)
    {
        GameManager.UpdateGameState(stateRequest);
        if(stateRequest == GameState.İNGAME)
        {
            GameManager.ChangeReadyClicks();
        }
    }
    [Command]
    public void CmdGMCardPlayed()
    {
        RpcGMCardPlayed();
    }
    [ClientRpc]
    void RpcGMCardPlayed()
    {
        GameManager.CardPlayed();
    }
    [Command]
    public void CmdTargetSelfCard()
    {
        TargetSelfCard();
    }
    [Command]
    public void CmdTargetOtherCard(GameObject target)
    {
        NetworkIdentity oppnentId = target.GetComponent<NetworkIdentity>();
        TargetOtherCard(oppnentId.connectionToClient);
    }
    [TargetRpc]
    void TargetSelfCard()
    {
        //Debug.Log("Targeted by self");
    }
    [TargetRpc]
    void TargetOtherCard(NetworkConnection target)
    {
        //Debug.Log("Targeted by other");
    }

    [Command]
    public void CmdSetAvailable()
    {
        RpcSetAvialable();
    }
    [ClientRpc]
    void RpcSetAvialable()

    {
        if (D00.transform.childCount > 0 && D01.transform.childCount > 0 && D02.transform.childCount > 0 &&
            D10.transform.childCount > 0 && D11.transform.childCount > 0 && D12.transform.childCount > 0 &&
            D20.transform.childCount > 0 && D21.transform.childCount > 0 && D22.transform.childCount > 0   )
        {
            isFull = true;
        }
        if (D00.transform.childCount == 0 || D01.transform.childCount == 0 || D02.transform.childCount == 0 ||
            D10.transform.childCount == 0 || D11.transform.childCount == 0 || D12.transform.childCount == 0 ||
            D20.transform.childCount == 0 || D21.transform.childCount == 0 || D22.transform.childCount == 0)
        {
            isFull = false;
        }


        if (!isFull)
        {
            foreach(GameObject x in dropZones)
            {
                IsHaveChild(x);
            }
            if  (D00.transform.childCount == 0 || D01.transform.childCount == 0 || D02.transform.childCount == 0)
            {
                CmdMakeUnavailable(D10); CmdMakeUnavailable(D11); CmdMakeUnavailable(D12);
                CmdMakeUnavailable(D20); CmdMakeUnavailable(D21); CmdMakeUnavailable(D22);
            }


            if (D10.transform.childCount == 0 || D11.transform.childCount == 0 || D12.transform.childCount == 0)
            {
                CmdMakeUnavailable(D20); CmdMakeUnavailable(D21); CmdMakeUnavailable(D22);
            }
        }
        else
        {
            foreach (GameObject x in dropZones)
            {
                IsHaveChild(x);
            }
        }

    }

    [Command(requiresAuthority =false)]
    public void CmdMakeAvailable(GameObject dropzone)
    {
        RpcMakeAvailable(dropzone);
    }

    [ClientRpc]
    public void RpcMakeAvailable(GameObject dropzone)
    {
        
        dropzone.tag = "Available";
        dropzone.GetComponent<Image>().color = Color.green;
    }

    [Command(requiresAuthority =false)]
    public void CmdMakeUnavailable(GameObject dropzone)
    {
        RpcMakeUnavailable(dropzone);
    }

    [ClientRpc]
    public void RpcMakeUnavailable(GameObject dropzone)
    {
        dropzone.tag = "Unavailable";
        dropzone.GetComponent<Image>().color = Color.red;
    }
    public void IsHaveChild(GameObject parent)
    {
        if (!isFull && parent.transform.childCount == 1)
        {
            CmdMakeUnavailable(parent);
        }
        else if(!isFull && parent.transform.childCount == 0)
        {
            CmdMakeAvailable(parent);
        }
        else if (isFull && parent.transform.childCount == 2)
        {
            CmdMakeUnavailable(parent);
        }
        else if (isFull && parent.transform.childCount == 1)
        {
            CmdMakeAvailable(parent);
        }
    }

    public GameObject GetCard(GameObject parent)
    {
        if (parent.transform.childCount > 0)
        {
            GameObject card = parent.transform.GetChild(0).gameObject;
            return card;
        }
        return null;
    }
    public GameObject GetSecondCard(GameObject parent)
    {
        if (parent.transform.childCount > 1)
        {
            GameObject card = parent.transform.GetChild(1).gameObject;
            return card;
        }
        return null;
    }
    public string GetName(GameObject card)
    {
        if (card)
        {
            return card.name;
        }
        else
        {
            return null;
        }
    }
    public void IsSame(GameObject card1, GameObject card2)
    {

        bool bool1 = true;
        bool bool2 = true;
        bool AvoidWrongTakes = true;

        if (card1 != null && card2 != null)
        {
            if ((card1.transform.parent.childCount == 2 && card1.transform.GetSiblingIndex() == 0) || (card2.transform.parent.childCount == 2 && card2.transform.GetSiblingIndex() == 0) && card1.transform.parent != card2.transform.parent)
            {
                AvoidWrongTakes = false;
            }
        }
        if (GetName(card1) == GetName(card2) && GetName(card1) != null && GetName(card2) != null && AvoidWrongTakes)
        {
            foreach(GameObject i in cardnames)
            {
                if(i == card1)
                {
                    bool1 = false;
                }
                if(i == card2)
                {
                    bool2 = false;
                }
            }
            if(bool1) cardnames.Add(card1);
            if(bool2) cardnames.Add(card2);   
        }
    }

    [Command]
    public void CmdCheckPoints()
    {
        RpcCheckPoints();
    }
    [ClientRpc]
    void RpcCheckPoints() 
    {
        C00 = GetCard(D00); Cc00 = GetSecondCard(D00);
        C01 = GetCard(D01); Cc01 = GetSecondCard(D01);
        C02 = GetCard(D02); Cc02 = GetSecondCard(D02);
        C10 = GetCard(D10); Cc10 = GetSecondCard(D10);
        C11 = GetCard(D11); Cc11 = GetSecondCard(D11);
        C12 = GetCard(D12); Cc12 = GetSecondCard(D12);
        C20 = GetCard(D20); Cc20 = GetSecondCard(D20);
        C21 = GetCard(D21); Cc21 = GetSecondCard(D21);
        C22 = GetCard(D22); Cc22 = GetSecondCard(D22);
       
        cardnames.Clear();
        GameObject[,] GameZone = new GameObject[3, 3] { {C00,C01,C02},{C10,C11,C12},{C20,C21,C22} };
        GameObject[,] GameZone2 = new GameObject[3, 3] { {Cc00,Cc01,Cc02}, {Cc10,Cc11,Cc12}, {Cc20,Cc21,Cc22} };
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 0)
                {
                    IsSame(GameZone[i, j], GameZone[i - 1, j]);
                }
                if(i != 2)
                {
                    IsSame(GameZone[i, j], GameZone[i + 1, j]);
                }
                if(j != 0)
                {
                    IsSame(GameZone[i, j], GameZone[i, j - 1]);
                }
                if (j != 2)
                {
                    IsSame(GameZone[i, j], GameZone[i, j + 1]);
                }
                IsSame(GameZone2[i, j], GameZone[i, j]);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 0)
                {
                    IsSame(GameZone2[i, j], GameZone2[i - 1, j]);
                }
                if (i != 2)
                {
                    IsSame(GameZone2[i, j], GameZone2[i + 1, j]);
                }
                if (j != 0)
                {
                    IsSame(GameZone2[i, j], GameZone2[i, j - 1]);
                }
                if (j != 2)
                {
                    IsSame(GameZone2[i, j], GameZone2[i, j + 1]);
                }
            }
        }
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (i != 0)
                {
                    IsSame(GameZone2[i, j], GameZone[i - 1, j]);
                }
                if (i != 2)
                {
                    IsSame(GameZone2[i, j], GameZone[i + 1, j]);
                }
                if (j != 0)
                {
                    IsSame(GameZone2[i, j], GameZone[i, j - 1]);
                }
                if (j != 2)
                {
                    IsSame(GameZone2[i, j], GameZone[i, j + 1]);
                }
                   
            }
        }

        if (cardnames.Count == 2)
        {
            CmdChangeHealth(0, 2);
        }
        else if (cardnames.Count > 2)
        {
            CmdChangeHealth(0, cardnames.Count * 2);
        }

        for (int i = 0; i < cardnames.Count; i++)
        {
            DestroyImmediate(cardnames[i]);
        }
       
        
      
    
    }
    [Command]
    public void CmdChangeHealth(int PlayerHitPoints, int OpponentHitPoints)
    {
        RpcChangeHealth(PlayerHitPoints, OpponentHitPoints);
    }
    [ClientRpc]
    void RpcChangeHealth(int PlayerHitPoints, int OpponentHitPoints)
    {
        GameManager.ChangeHealth(PlayerHitPoints, OpponentHitPoints, hasAuthority);
    }
    [Command]
    public void CmdChangeOnceToTrue()
    {
        RpcChangeOnceToTrue();
    }
    [ClientRpc]
    void RpcChangeOnceToTrue()
    {
        GameManager.once = true;
    }
}
