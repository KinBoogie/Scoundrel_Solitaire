using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("HP Settings")]
    [SerializeField] private int maxHp = 20;
    [SerializeField] private Text hpText;        // Assign HP text in SettingsBar

    [Header("References")]
    [SerializeField] private DrawManager drawManager;   // Assign your DrawManager (DeckArea)
    [SerializeField] private WeaponStack weaponStack;   // Assign your WeaponArea object (with WeaponStack)

    private int currentHp;

    // Shield state
    private CardData currentShield;
    private int shieldRemaining;
    private int lastEnemyValueOnShield;

    private void Start()
    {
        currentHp = maxHp;
        UpdateHpUI();
    }

    public int MaxHp
    {
        get => maxHp;
        set
        {
            maxHp = Mathf.Max(1, value);
            currentHp = Mathf.Min(currentHp, maxHp);
            UpdateHpUI();
        }
    }

    private void UpdateHpUI()
    {
        if (hpText != null)
            hpText.text = $"{currentHp}/{maxHp}";
    }

    /// <summary>
    /// Called by DrawManager when a room card was clicked.
    /// </summary>
    public void HandleCardClickFromRoom(CardData card, GameObject cardInstance)
    {
        if (card == null) return;

        Debug.Log($"GameManager: HandleCardClickFromRoom for {card.name} ({card.suit} {card.rank})");

        // Apply rules
        if (card.IsEnemy)
        {
            HandleEnemy(card);
        }
        else if (card.IsShield)
        {
            HandleShield(card);
        }
        else if (card.IsPotion)
        {
            HandlePotion(card);
        }

        // Remove the visual from the room immediately
        if (cardInstance != null)
        {
            Debug.Log("GameManager: Destroying clicked card instance from room.");
            Destroy(cardInstance);
        }

        // Tell DrawManager to update room (refill if needed, check win)
        if (drawManager != null)
        {
            drawManager.OnRoomCardResolved();
        }

        // Check lose condition
        if (currentHp <= 0)
        {
            HandlePlayerDefeated();
        }
    }

    // ========== ENEMY (Spades / Clubs) ==========

    private void HandleEnemy(CardData enemy)
    {
        int enemyValue = enemy.Value;
        bool shieldUsed = false;
        int damageToPlayer = enemyValue;

        Debug.Log($"GameManager: Enemy hit for value {enemyValue}");

        if (currentShield != null && CanShieldAbsorb(enemyValue))
        {
            shieldUsed = true;
            lastEnemyValueOnShield = enemyValue;

            int shieldBefore = shieldRemaining;
            shieldRemaining -= enemyValue;

            if (shieldRemaining < 0)
            {
                damageToPlayer = -shieldRemaining;   // leftover to player
                shieldRemaining = 0;
            }
            else
            {
                damageToPlayer = 0;
            }

            Debug.Log($"GameManager: Shield absorbed. Before={shieldBefore}, After={shieldRemaining}, damageToPlayer={damageToPlayer}");

            // Visually stack this enemy onto the current shield
            if (weaponStack != null)
            {
                weaponStack.AddCardFromData(enemy);
            }

            // Shield depleted? clear it.
            if (shieldRemaining <= 0)
            {
                ClearShield();
            }
        }

        if (!shieldUsed)
        {
            // Direct hit to player
            currentHp -= damageToPlayer;
            currentHp = Mathf.Max(0, currentHp);
            Debug.Log($"GameManager: Direct damage to player: {damageToPlayer}, HP now {currentHp}");
            UpdateHpUI();
        }
    }

    private bool CanShieldAbsorb(int enemyValue)
    {
        if (currentShield == null || shieldRemaining <= 0)
            return false;

        // First enemy can always go to shield
        if (lastEnemyValueOnShield <= 0)
            return true;

        // Rule: shield can take enemies with rank LOWER or EQUAL to last one it took
        return enemyValue <= lastEnemyValueOnShield;
    }

    // ========== SHIELD (Diamonds) ==========

    private void HandleShield(CardData shieldCard)
    {
        Debug.Log($"GameManager: New shield selected: {shieldCard.name} value {shieldCard.Value}");

        // Picking a new shield: discard previous shield + its enemies
        ClearShield();

        currentShield = shieldCard;
        shieldRemaining = shieldCard.Value;
        lastEnemyValueOnShield = 0;

        // Visual: show this shield in the weapon area
        if (weaponStack != null)
        {
            weaponStack.AddCardFromData(shieldCard);
        }
    }

    private void ClearShield()
    {
        if (currentShield != null)
        {
            Debug.Log("GameManager: Clearing shield and its stacked enemies.");
        }

        currentShield = null;
        shieldRemaining = 0;
        lastEnemyValueOnShield = 0;

        if (weaponStack != null)
        {
            weaponStack.ClearAll();   // remove all visuals (shield + absorbed enemies)
        }
    }

    // ========== POTION (Hearts) ==========

    private void HandlePotion(CardData potion)
    {
        int heal = potion.Value;
        int before = currentHp;
        currentHp = Mathf.Min(maxHp, currentHp + heal);
        Debug.Log($"GameManager: Potion heal {heal}. HP {before} -> {currentHp}");
        UpdateHpUI();
    }

    // ========== END CONDITIONS ==========

    private void HandlePlayerDefeated()
    {
        Debug.Log("GameManager: Player defeated! HP reached 0.");
        // TODO: show game over UI
    }

    public void HandleWin()
    {
        Debug.Log("GameManager: You win! Deck cleared and room empty.");
        // TODO: show win UI
    }
}
