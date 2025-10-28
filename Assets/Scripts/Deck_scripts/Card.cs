using System;
using System.Reflection;
using System.Runtime.CompilerServices;
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
    private Transform originalParent;
    public Transform newParent;
    void OnValidate()
    {
        Image image = this.GetComponent<Image>();
        image.color = this.color;
    }
    void Update()
    {
        if(withinDeck)
        {
            if (isCardSlot)
            {
                Image image = this.GetComponent<Image>();
                image.color = Color.white;
            }
            else
            {
                Image image = this.GetComponent<Image>();
                image.color = color;
            }
        }
        else
        {
            if (isCardSlot)
            {
                Image image = this.GetComponent<Image>();
                image.color = Color.black;
            }
            else
            {
                Image image = this.GetComponent<Image>();
                image.color = color;
            }
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        orginalPostion = transform.position;
        originalParent = transform.parent;
        Debug.Log("Dragging");
        if (isCardSlot)
        {
            //Card is locked or empty card slot
            return;
        }
        transform.SetParent(newParent, true);
        transform.SetAsLastSibling();

    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (!isCardSlot)
        {
            transform.position = eventData.position;
        }

    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        GameObject overlapped = this.GetComponentInParent<UIOverlapChecker>().checkOverlap(this.gameObject);
        if (overlapped)
        {
            Debug.Log(overlapped.name + "had been intersected");
            if (overlapped.GetComponent<Card>().isCardSlot && isCardSlot)
            {
                //do nothing
            }
            else if (!overlapped.GetComponent<Card>().withinDeck && !withinDeck)
            {
                //do nothing
            }
            else if (overlapped.GetComponent<Card>().withinDeck && withinDeck)
            {
                Debug.Log("Exchange");
                this.ExchangeTo(overlapped.GetComponent<Card>());
            }
            else if (overlapped.GetComponent<Card>().withinDeck)
            {
                this.GetComponent<Card>().CopyTo(overlapped.GetComponent<Card>());
            }else if (withinDeck && !overlapped.GetComponent<Card>().withinDeck)
            {
                EmptyCard();
            }
        }
        transform.position = orginalPostion;
        transform.SetParent(originalParent, true);
        //call parent overlap checker
        //check which card slot it intercepts and place the card in slot intercepts
    }
    void CopyTo(Card card)
    {
        //TODO check wether the card is already within the deck.
        if (card.withinDeck && !this.withinDeck)
        {
            card.id = this.id;
            card.color = this.color;
            card.isCardSlot = false;
        }
    }
    void ExchangeTo(Card card)
    {
        Image image1 = this.GetComponent<Image>();
        Image image2 = card.GetComponent<Image>();
        int id1 = this.id;
        int id2 = card.id;
        Color color1 = color;
        Color color2 = card.color;
        bool isCardSlot1 = isCardSlot;
        bool withinDeck1 = withinDeck;
        card.id = id1;
        card.color = color1;
        this.id = id2;
        this.color = color2;
        isCardSlot = card.isCardSlot;
        withinDeck = card.withinDeck;
        card.withinDeck = withinDeck1;
        card.isCardSlot = isCardSlot1;
    }
    
    void EmptyCard()
    {
        this.withinDeck = true;
        this.isCardSlot = true;
    }
}
