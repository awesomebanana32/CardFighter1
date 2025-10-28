using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DeckManger : MonoBehaviour
{
    public int[] cards;
    public GameObject card;
    private long deck;
    [SerializeField]
    private Transform parent;
    [SerializeField]
    private bool isDeck;
    private float layoutSpacing = 20f;
    public void Start()
    {
        CampaignData data = SaveManager.LoadGame();
        deck = data.deck;
        Vector3 position = transform.position;
        for(int i = 0; i < 16; i++)
        {
            GameObject row = Instantiate(new GameObject("tkdsflsj;"), position, Quaternion.identity, transform);
            RectTransform rectTransform = row.AddComponent<RectTransform>();

            rectTransform.sizeDelta = transform.parent.GetComponent<RectTransform>().sizeDelta;
            rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, card.GetComponent<RectTransform>().sizeDelta.y);
            Transform newPos = row.transform;
            newPos.position += new Vector3(240,0,0);
            for (int j = 0; j < 4; j++)
            {
                newPos.position -= new Vector3(card.GetComponent<RectTransform>().rect.width + layoutSpacing,0,0);
                Debug.Log(card.GetComponent<RectTransform>().rect.width + layoutSpacing);
                GameObject newCard = Instantiate(card, position, Quaternion.identity, newPos);
                newCard.GetComponent<Card>().newParent = parent;
                newCard.GetComponent<Card>().withinDeck = false;
                //TODO: check if the card id unlocked
                newCard.GetComponent<Card>().isCardSlot = false;
            }
        }
    }

    public void LoadCardDatabase(int level)
    {
        // Load the database with Cool Cards
    }
    public void SetCard(int index, int cardId)
    {
        if (((deck >> (cardId - 1)) & 1) == 1)
        {
            cards[index] = cardId;
        }
        else
        {
            Debug.Log("You do not have that card");
        }
    }
}
