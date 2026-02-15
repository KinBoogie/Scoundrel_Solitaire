using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Scoundrel/Card")]
public class CardData : ScriptableObject
{
    public enum Suit
    {
        Clubs,
        Diamonds,
        Hearts,
        Spades
    }

    [Header("Card definition")]
    public Suit suit;

    [Tooltip("2–10, J=11, Q=12, K=13, A=14")]
    [Range(2, 14)]
    public int rank;

    [Header("Sprites")]
    public Sprite frontSprite;
    public Sprite backSprite;

    // Convenience helpers based on your rules
    public bool IsEnemy => suit == Suit.Spades || suit == Suit.Clubs;
    public bool IsShield => suit == Suit.Diamonds;
    public bool IsPotion => suit == Suit.Hearts;

    public int Value => rank;   // enemy damage, shield value, heal amount
}
