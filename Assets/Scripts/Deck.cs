using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Deck : NetworkBehaviour
{
    [SyncVar]
    public int ReadyClients = 0;

    public SyncList<GameObject> deck = new SyncList<GameObject>();
}
