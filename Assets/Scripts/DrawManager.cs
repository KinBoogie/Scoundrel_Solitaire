using UnityEngine;
using UnityEngine.UI;

public class DrawManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Deck deck;
    [SerializeField] private GameManager gameManager;

    [Header("Room setup")]
    [SerializeField] private Transform[] roomSlots;   // 4 slots in the UI
    [SerializeField] private GameObject cardPrefab;   // Card UI prefab with CardDisplay + Button

    private void Start()
    {
        Debug.Log("DrawManager.Start: InitialDeal()");
        InitialDeal();
    }

    private void InitialDeal()
    {
        if (deck == null)
        {
            Debug.LogError("DrawManager: Deck reference not assigned.");
            return;
        }

        if (roomSlots == null || roomSlots.Length == 0)
        {
            Debug.LogError("DrawManager: roomSlots not assigned.");
            return;
        }

        if (cardPrefab == null)
        {
            Debug.LogError("DrawManager: cardPrefab not assigned.");
            return;
        }

        // At game start: fill room up to 4 cards
        foreach (Transform slot in roomSlots)
        {
            if (!deck.HasCards) break;
            DrawOneCardIntoSlot(slot);
        }
    }

    private void DrawOneCardIntoSlot(Transform slot)
    {
        if (slot == null)
            return;

        if (slot.childCount > 0)
            return; // already occupied

        CardData data = deck.DrawCard();
        if (data == null)
        {
            Debug.Log("DrawManager: Tried to draw but deck empty.");
            return;
        }

        GameObject inst = Instantiate(cardPrefab, slot, false);
        Debug.Log($"DrawManager: Spawned card '{data.name}' in slot '{slot.name}'.");

        // Reset local position & scale
        RectTransform rt = inst.GetComponent<RectTransform>();
        if (rt != null)
        {
            rt.anchoredPosition = Vector2.zero;
            rt.localScale = Vector3.one;
        }
        else
        {
            inst.transform.localPosition = Vector3.zero;
            inst.transform.localScale = Vector3.one;
        }

        // Setup visuals
        CardDisplay display = inst.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.Setup(data);
            display.ShowFrontImmediate();   // show front in the room
        }
        else
        {
            Debug.LogWarning("DrawManager: Spawned card has no CardDisplay component.");
        }

        // Click handling via Button (root or children)
        Button btn = inst.GetComponent<Button>();
        if (btn == null)
            btn = inst.GetComponentInChildren<Button>();

        if (btn != null)
        {
            Debug.Log("DrawManager: Found Button on card, adding click listener.");
            btn.onClick.RemoveAllListeners();   // safety
            btn.onClick.AddListener(() =>
            {
                OnRoomCardClicked(inst, data);
            });
        }
        else
        {
            Debug.LogWarning("DrawManager: card prefab has no Button component; click won't work.");
        }
    }

    private void OnRoomCardClicked(GameObject cardInstance, CardData data)
    {
        Debug.Log($"DrawManager: OnRoomCardClicked fired for '{data.name}'.");

        if (gameManager != null)
        {
            gameManager.HandleCardClickFromRoom(data, cardInstance);
            // GameManager will destroy the cardInstance and then call OnRoomCardResolved()
        }
        else
        {
            // Fallback: just destroy card and free slot
            if (cardInstance != null)
                Destroy(cardInstance);

            OnRoomCardResolved();
        }
    }

    /// <summary>
    /// Called by GameManager after it processed a clicked card and destroyed its visual,
    /// or by DrawManager itself in the fallback path.
    /// </summary>
    public void OnRoomCardResolved()
    {
        int remaining = CountCardsInRoom();
        Debug.Log($"DrawManager: OnRoomCardResolved, remaining in room = {remaining}, deck.HasCards = {deck.HasCards}");

        // Win condition: no cards in deck AND no cards in room
        if (!deck.HasCards && remaining == 0)
        {
            if (gameManager != null)
                gameManager.HandleWin();

            return;
        }

        // Room rule: When there is exactly 1 card left in the room and
        // deck still has cards, draw 3 new cards into empty slots.
        if (remaining == 1 && deck.HasCards)
        {
            RefillRoom();
        }
    }

    private int CountCardsInRoom()
    {
        int count = 0;
        foreach (Transform slot in roomSlots)
        {
            if (slot == null) continue;
            count += slot.childCount;
        }
        return count;
    }

    private void RefillRoom()
    {
        Debug.Log("DrawManager: RefillRoom called.");

        foreach (Transform slot in roomSlots)
        {
            if (!deck.HasCards)
                break;

            if (slot == null) continue;

            if (slot.childCount == 0)
            {
                DrawOneCardIntoSlot(slot);
            }
        }
    }
}
