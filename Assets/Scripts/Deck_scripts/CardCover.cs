using UnityEngine;

[CreateAssetMenu(fileName = "CardCover", menuName = "Scriptable Objects/CardCover")]
public class CardCover : ScriptableObject
{
    public int id;
    public string cardName;
    public Sprite cardCover;
}
