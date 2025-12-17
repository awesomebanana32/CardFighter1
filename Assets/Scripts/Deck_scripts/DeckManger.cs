using System;
using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DeckManger : MonoBehaviour
{
    private int[] cards;
    public GameObject card;
    private long deck;
    [SerializeField]
    private Transform parent;
    [SerializeField]
    public float layoutSpacing;
    public Sprite defaultCover;
    public ObjectDatabaseSO cardDatabase;
    public void Start()
    {
        cards = new int[8];
        for(int i = 0; i < 8; i++)
        {
            cards[i] = -1;
        }
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
            newPos.position += new Vector3(0,0,0);
            for (int j = 0; j < 4; j++)
            {
                int currentId = 4 * i + j;
                newPos.position -= new Vector3(card.GetComponent<RectTransform>().rect.width + layoutSpacing,0,0);
                GameObject newCard = Instantiate(card, position, Quaternion.identity, newPos);
                newCard.GetComponent<Card>().newParent = parent;
                newCard.GetComponent<Card>().withinDeck = false;
                newCard.GetComponent<Card>().id = 4 * i + j;
                if (!IsInDeck(currentId))
                {
                    newCard.GetComponent<Card>().isCardSlot = true;
                }
                else
                {
                    //TODO: retrieve the card cover.
                    Sprite cover = cardDatabase.GetCardCoverByID(currentId);
                    //TODO: apply affects
                    if (cover != null)
                    {
                        newCard.GetComponent<Image>().sprite = cover;
                    }
                    else
                    {
                        newCard.GetComponent<Image>().sprite = defaultCover;
                    }
                }

            }
        }
    }

    /*
    SetCard
        When a unlocked card is dragged onto a card in the deck, we set the card in decks array after checking it is unlocked.
    */
    public bool SetCard(int index, int cardId)
    {
        if (IsInDeck(cardId) && !IsInPlayingDeck(cardId))
        {
            cards[index] = cardId;
            return true;
        }
        return false;
    }
    public bool IsInPlayingDeck(int cardId)
    {
        for (int i = 0; i < 8; i++)
        {
            if (cards[i] == cardId)
            {
                return true;
            }

        }
        return false;
    }
    public bool IsInDeck(int cardId)
    {
        return (((deck >> (cardId - 1)) & 1) == 1);
    }
}
