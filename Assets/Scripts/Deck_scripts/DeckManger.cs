using System;
using UnityEditor.Rendering;
using UnityEngine;

[Serializable]
public class DeckManger : MonoBehaviour
{
    public int[] cards;
    private long deck;
    public void Start()
    {
        CampaignData data = SaveManager.LoadGame();
        deck = data.deck;
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
