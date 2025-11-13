using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Scriptable Objects/CardDatabase")]
public class CardDatabase : ScriptableObject
{
    public List<CardCover> cardCovers;
    public Sprite GetSprite(int id)
    {
        CardCover data = cardCovers.FirstOrDefault(s => s.id == id);
        if (data != null)
        {
            return data.cardCover;
        }
        return null;
    }
    
}
