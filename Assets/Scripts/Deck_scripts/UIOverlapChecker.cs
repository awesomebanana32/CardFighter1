using System;
using Unity.VisualScripting;
using UnityEngine;

public class UIOverlapChecker : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private float intersect(GameObject a, GameObject b)
    {
        if (a == null || b == null)
        {
            return 0.0f;
        }
        if (a.GetComponent<Card>() == null || b.GetComponent<Card>() == null)
        {
            return 0.0f;
        }
        Rect rectA = a.GetComponent<RectTransform>().rect;
        Rect rectB = b.GetComponent<RectTransform>().rect;
        float leftSide = Math.Max(rectA.x, rectB.x);
        float topSide = Math.Max(rectA.y, rectB.y);
        float rightSide = Math.Min(rectA.x+rectA.width, rectB.x+rectB.width);
        float bottomSide = Math.Min(rectA.y + rectA.height, rectB.y + rectB.height);
        if (rightSide - leftSide < 0f)
        {
            return 0.0f;
        }
        if (bottomSide - topSide < 0f)
        {
            return 0f;
        }
        return (rightSide-leftSide) * (bottomSide - topSide);
    }
    public GameObject checkOverlap(GameObject heldCard)
    {
        Transform[] cards = this.GetComponentsInChildren<Transform>();
        float maxInterSection = 0.0f;
        GameObject resCard = cards[0].gameObject;
        foreach (Transform card in cards)
        {
            if (card.gameObject == heldCard) continue;
            float intersection = intersect(card.gameObject, heldCard);
            if (intersection > maxInterSection)
            {
                maxInterSection = intersection;
                resCard = card.gameObject;
            }
        }
        if(maxInterSection > 0f)
        {
            return resCard;
        }
        return null;
    }
}
