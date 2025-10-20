using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id;
    [SerializeField]
    public Color color;
    [SerializeField]
    public bool withinDeck;
    [SerializeField]
    public bool isCardSlot;
    private Vector3 orginalPostion;
    void OnValidate()
    {
        Image image = this.GetComponent<Image>();
        image.color = this.color;
    }
    void Start()
    {
        if(withinDeck)
        {
            if (isCardSlot)
            {
                //show an empty card face     
            }
            else
            {

            }
        }
        else
        {
            if (isCardSlot)
            {
                //show an locked card face
            }
            else
            {
                //show the unlocked card based on the id
            }
        }
    }

    // Update is called once per frame
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        orginalPostion = transform.position;
        if (isCardSlot)
        {
            return;
        }
        if (withinDeck)
        {
            //
        }
        transform.SetAsLastSibling();

    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
        //add some highlighting to the edge of the card

    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("DragEnded");
        transform.position = orginalPostion;
        GameObject overlapped = this.GetComponentInParent<UIOverlapChecker>().checkOverlap(this.gameObject);
        if (overlapped)
        {
            Debug.Log( this.name + "Copy the red card into the white card" + overlapped.name);
            //this.CopyTo(overlapped.GetComponent<Card>());
            overlapped.GetComponent<Card>().CopyTo(this);
        }
        //call parent overlap checker
        //check which card slot it intercepts and place the card in slot intercepts
    }
    void CopyTo(Card card)
    {
        this.color = card.color;
        this.isCardSlot = card.isCardSlot;
        Image image = this.GetComponent<Image>();
        image.color = this.color;
    }
}
