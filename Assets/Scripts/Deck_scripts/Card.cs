/*
Card.cs
8coolguy
    This Class hold information on the Card. There are two identifying variables to decide where in the UI this card is. This Card is either part of the deck or not. It is also a card slot or not.
    inDeck and cardSlot => empty deck slot
    not inDeck and cardSlot => locked card
    indeck and not cardSlot => filled deck slot
    not inDeck and not cardSlot => unlocked Card
*/

using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters;
using System.Security.Cryptography;
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
    [SerializeField]
    private Sprite defualtCover;
    [SerializeField]
    private Sprite defaultLockScreen;
    public GameObject deck;
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
                image.sprite = defualtCover;
            }
            else
            {
                //TODO: show card cover
                Image image = this.GetComponent<Image>();
            }
        }
        else
        {
            if (isCardSlot)
            {
                Image image = this.GetComponent<Image>();
                image.sprite = defaultLockScreen;
            }
            else
            {
                //TODO: show card cover
                Image image = this.GetComponent<Image>();
            }
        }
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        orginalPostion = transform.position;
        originalParent = transform.parent;
        if (isCardSlot)
        {
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
            if (overlapped.GetComponent<Card>().withinDeck && withinDeck)
            {
                Sprite image = overlapped.GetComponent<Image>().sprite;
                overlapped.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                this.GetComponent<Image>().sprite = image;
                this.ExchangeTo(overlapped.GetComponent<Card>());
            }
            else if (overlapped.GetComponent<Card>().withinDeck)
            {
                if (overlapped.GetComponent<Card>().deck.GetComponent<DeckManger>().SetCard(overlapped.GetComponent<Card>().id, id))
                {
                    overlapped.GetComponent<Image>().sprite = this.GetComponent<Image>().sprite;
                    this.GetComponent<Card>().CopyTo(overlapped.GetComponent<Card>());
                }
            }else if (withinDeck && !overlapped.GetComponent<Card>().withinDeck)
            {
                EmptyCard();
            }
        }
        transform.position = orginalPostion;
        transform.SetParent(originalParent, true);
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
        this.GetComponent<Image>().sprite = null;
        this.withinDeck = true;
        this.isCardSlot = true;
    }
}
