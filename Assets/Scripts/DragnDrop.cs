using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class DragnDrop : NetworkBehaviour
{
    public GameObject Canvas;
    public PlayerManager PlayerManager;
    

    private bool isDragging = false;
    private bool isOverDropZone = false;
    private bool isDraggable = true;
    private GameObject startParent;
    private Vector2 startPosition;
    private GameObject dropZone;
    public PlayerManager gameState;

    void Start()
    {
        Canvas = GameObject.Find("Main Canvas");
        if (!hasAuthority)
        {
            isDraggable = false;
        }
        NetworkIdentity networkIdentity = NetworkClient.connection.identity;
        PlayerManager = networkIdentity.GetComponent<PlayerManager>();

    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        isOverDropZone = true;
        dropZone = collision.gameObject;
        if (dropZone.tag == "Available")
        {
            dropZone.GetComponent<Image>().color = Color.red;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        isOverDropZone = false;
        if (dropZone.tag == "Available")
        {
            dropZone.GetComponent<Image>().color = Color.green;
        }
        dropZone = null;

    }
    public void StartDragging()
    {
       
        if (!hasAuthority)
        {
            isDraggable = false;
        }
        if (!isDraggable) return;
        isDragging = true;
        startParent = transform.parent.gameObject;
        startPosition = transform.position;
    }
    public void EndDragging()
    {

        if (!isDraggable) return;
        isDragging = false;
        if (dropZone != null)
        {
            if (dropZone.tag == "Unavailable")
            {
                isOverDropZone = false;
            }
        }
        if (isOverDropZone && PlayerManager.isMyTurn)
        {
            isDraggable = false;
            PlayerManager.PlayCard(gameObject, dropZone);
            PlayerManager.CmdSetAvailable();
           // PlayerManager.CmdCheckAvailables();
           
           
        }
        else
        {
            transform.position = startPosition;
            transform.SetParent(startParent.transform, false);
        }
       
    }
    void Update()
    {
        if(isDragging)
        {
            transform.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            transform.SetParent(Canvas.transform, true);   
        }
    }
}
