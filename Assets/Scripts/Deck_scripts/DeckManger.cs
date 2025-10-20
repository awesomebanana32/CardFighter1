using System;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class DeckManger : MonoBehaviour
{
    public int[] cards;
    public GameObject card;
    private long deck;
    public void Start()
    {
        CampaignData data = SaveManager.LoadGame();
        deck = data.deck;

        Vector3 position = transform.position;
        //position.x -= 350;
        //position.y += 400;
        for(int i = 0; i < 16; i++)
        {
            //position.y -= 200;
            GameObject row = Instantiate(new GameObject(), position, Quaternion.identity, transform);
            RectTransform rectTransform = row.AddComponent<RectTransform>();
            rectTransform.sizeDelta = new Vector2(300f, 100f);
            HorizontalLayoutGroup layout = row.AddComponent<HorizontalLayoutGroup>();
            layout.spacing = 20;
            for (int j = 0; j < 4; j++)
            {
                GameObject newCard = Instantiate(card, position, Quaternion.identity, row.transform);
            }
            //position.x -= 640;
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
