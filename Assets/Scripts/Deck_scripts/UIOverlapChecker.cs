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
            return 0f;
        }
        Vector3[] cornersA = new Vector3[4];
        Vector3[] cornersB = new Vector3[4];
        // This method populates the 'corners' array with world space coordinates.
        RectTransform rectA = a.GetComponent<RectTransform>();
        RectTransform rectB = b.GetComponent<RectTransform>();
        rectA.GetWorldCorners(cornersA);
        rectB.GetWorldCorners(cornersB);
        float leftSide = Math.Max(cornersA[0].x, cornersB[0].x);
        float topSide = Math.Max(cornersA[0].y, cornersB[0].y);
        float rightSide = Math.Min(cornersA[2].x,cornersB[2].x);
        float bottomSide = Math.Min(cornersA[2].y, cornersB[2].y);
        if (rightSide - leftSide < 0f)
        {
            return 0f;
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
            if (card.GetComponent<Card>() == null) continue;
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
