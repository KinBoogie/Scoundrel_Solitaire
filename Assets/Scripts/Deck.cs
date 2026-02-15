using UnityEngine;
using System.Collections.Generic;

public class Deck : MonoBehaviour
{
    [Header("All cards in the deck (52)")]
    public List<CardData> allCards = new List<CardData>();

    private List<CardData> runtimeDeck = new List<CardData>();

    private void Awake()
    {
        BuildAndShuffleDeck();
    }

    public void BuildAndShuffleDeck()
    {
        runtimeDeck.Clear();
        runtimeDeck.AddRange(allCards);
        Shuffle(runtimeDeck);
    }

    public bool HasCards => runtimeDeck.Count > 0;

    public CardData DrawCard()
    {
        if (runtimeDeck.Count == 0)
            return null;

        int lastIndex = runtimeDeck.Count - 1;
        CardData c = runtimeDeck[lastIndex];
        runtimeDeck.RemoveAt(lastIndex);
        return c;
    }

    private void Shuffle(List<CardData> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            int j = Random.Range(i, list.Count);
            CardData temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
