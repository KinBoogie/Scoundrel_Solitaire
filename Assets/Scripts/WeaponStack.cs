using UnityEngine;

public class WeaponStack : MonoBehaviour
{
    [Header("Visual slots (where stacked card visuals appear)")]
    public Transform[] slots;         // assign at least one slot Transform

    [Header("Card visual prefab (UI)")]
    public GameObject cardPrefab;     // assign your card prefab (must have CardDisplay)

    [Header("Stack offset when stacking multiple on one weapon (x right, y down)")]
    public Vector2 offsetPerCard = new Vector2(36f, -18f);

    public void AddCardFromData(CardData data)
    {
        if (cardPrefab == null)
        {
            Debug.LogError("WeaponStack: cardPrefab not assigned.");
            return;
        }

        if (slots == null || slots.Length == 0 || slots[0] == null)
        {
            Debug.LogError("WeaponStack: no slots assigned.");
            return;
        }

        Transform target = slots[0];

        // Instantiate visual under the slot
        GameObject inst = Instantiate(cardPrefab, target, false);

        RectTransform rt = inst.GetComponent<RectTransform>();
        int indexInStack = target.childCount - 1; // 0 for first, 1 for second, etc.

        if (rt != null)
        {
            rt.anchoredPosition = offsetPerCard * Mathf.Max(0, indexInStack);
        }

        CardDisplay display = inst.GetComponent<CardDisplay>();
        if (display != null)
        {
            display.Setup(data);
            display.FlipToFront(); // reveal immediately with animation
        }
    }

    public void ClearAll()
    {
        if (slots == null) return;

        foreach (Transform s in slots)
        {
            if (s == null) continue;

            for (int i = s.childCount - 1; i >= 0; i--)
            {
                Destroy(s.GetChild(i).gameObject);
            }
        }
    }
}
