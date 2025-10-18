using System;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
            Debug.Log(overlapped.name);
        }
        //call parent overlap checker
        //check which card slot it intercepts and place the card in slot intercepts
    }
}
