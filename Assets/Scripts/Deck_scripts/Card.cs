using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IDropHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int id;
    [SerializeField]
    private String _name;
    [SerializeField]
    private bool withinDeck;
    [SerializeField]
    private bool isCardSlot;
    private Vector3 orginalPostion;
    void Start()
    {
        this.name = _name;
        if (withinDeck)
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
        if (isCardSlot)
        {
            return;
        }
        if (withinDeck)
        {
            //
        }

    }
    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        Debug.Log("Dragging");
        //if not withinDeck
        //create duplicate that follows the cursor wherever it goes
        //add some highlighting to the edge of the card
    }
    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("DragEnded");
        //check which card slot it intercepts and place the card in slot intercepts
    }
    void IDropHandler.OnDrop(PointerEventData eventData)
    {
        Debug.Log("Dropped");
    }
}
