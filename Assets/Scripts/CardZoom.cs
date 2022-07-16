using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CardZoom : NetworkBehaviour
{

    public GameObject Canvas;
    public GameObject ZoomCard;

    private GameObject zoomCard;
    private Sprite zoomSprite;
    private bool isZoomable;

    public void Awake()
    {
        Canvas = GameObject.Find("Main Canvas");
        zoomSprite = gameObject.GetComponent<Image>().sprite;

    }

    public void OnMouseClick()
    {
        if (Input.GetMouseButtonUp(1))
        {
            if (hasAuthority)
            {
                gameObject.tag = "Zoomable";
            }
            if (gameObject.tag != "Zoomable") return; 
            if (GameObject.Find("ZoomCard(Clone)") != null)  return; 

            if (Input.mousePosition.y + 599 <= 1080)
            {
                zoomCard = Instantiate(ZoomCard, new Vector2(Input.mousePosition.x, Input.mousePosition.y + 250), Quaternion.identity);
            }
            else
            {
                zoomCard = Instantiate(ZoomCard, new Vector2(Input.mousePosition.x, Input.mousePosition.y - 250), Quaternion.identity);
            }
            zoomCard.GetComponent<Image>().sprite = zoomSprite;
            zoomCard.transform.SetParent(Canvas.transform, true);
            RectTransform rect = zoomCard.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(240, 344);
        }
    }
    public void OnHoverExit()
    {
        Destroy(zoomCard);
    }
}
