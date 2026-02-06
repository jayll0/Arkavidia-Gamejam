using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Manager untuk koordinasi semua UI battle
/// Attach ke GameObject BattleCanvas atau UIManager
/// </summary>
public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance { get; private set; }

    [Header("Character Status Panels")]
    [SerializeField] private List<CharacterStatusUI> characterPanels = new List<CharacterStatusUI>();

    [Header("Enemy Status Panels")]
    [SerializeField] private List<EnemyStatusUI> enemyPanels = new List<EnemyStatusUI>();

    [Header("Action Buttons")]
    [SerializeField] private ActionButtonPanel actionButtonPanel;

    [Header("Auto-Setup")]
    [SerializeField] private bool autoLinkCharacters = true; // Auto link saat Start

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Get ActionButtonPanel if not assigned
        if (actionButtonPanel == null)
        {
            actionButtonPanel = FindObjectOfType<ActionButtonPanel>();
        }

        if (autoLinkCharacters)
        {
            // Delay sedikit untuk memastikan BattleManager sudah init
            Invoke(nameof(AutoLinkCharactersToUI), 0.6f);
        }
    }

    /// <summary>
    /// Auto-link characters dari BattleManager ke UI panels
    /// </summary>
    private void AutoLinkCharactersToUI()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogWarning("BattleManager not found! Cannot auto-link characters.");
            return;
        }

        // Link Players
        var players = BattleManager.Instance.playerCharacters;
        for (int i = 0; i < players.Count && i < characterPanels.Count; i++)
        {
            if (characterPanels[i] != null && players[i] != null)
            {
                characterPanels[i].SetCharacter(players[i]);
                Debug.Log($"Linked {players[i].gameObject.name} to Character Panel {i + 1}");
            }
        }

        // Link Enemies
        var enemies = BattleManager.Instance.enemyCharacters;
        for (int i = 0; i < enemies.Count && i < enemyPanels.Count; i++)
        {
            if (enemyPanels[i] != null && enemies[i] != null)
            {
                enemyPanels[i].SetEnemy(enemies[i]);
                Debug.Log($"Linked {enemies[i].gameObject.name} to Enemy Panel {i + 1}");
            }
        }

        Debug.Log("Auto-link characters to UI completed!");
    }

    /// <summary>
    /// Manual link specific character ke panel
    /// </summary>
    public void LinkCharacterToPanel(BattleMovement character, int panelIndex)
    {
        if (panelIndex >= 0 && panelIndex < characterPanels.Count)
        {
            characterPanels[panelIndex].SetCharacter(character);
        }
        else
        {
            Debug.LogWarning($"Panel index {panelIndex} out of range!");
        }
    }

    /// <summary>
    /// Manual link specific enemy ke panel
    /// </summary>
    public void LinkEnemyToPanel(EnemyBattleMovement enemy, int panelIndex)
    {
        if (panelIndex >= 0 && panelIndex < enemyPanels.Count)
        {
            enemyPanels[panelIndex].SetEnemy(enemy);
        }
        else
        {
            Debug.LogWarning($"Panel index {panelIndex} out of range!");
        }
    }

    /// <summary>
    /// Handle attack button click - dipanggil oleh ActionButtonPanel atau langsung dari UI
    /// </summary>
    public void OnAttackButtonClick()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogWarning("BattleManager not found!");
            return;
        }

        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();

        if (currentEntity == null || !currentEntity.isPlayer)
        {
            Debug.LogWarning("Not player's turn!");
            return;
        }

        // Check if there are enemies
        if (BattleManager.Instance.enemyCharacters.Count == 0)
        {
            Debug.LogWarning("No enemies to attack!");
            return;
        }

        // Enter enemy selection mode
        BattleManager.Instance.state = BattleManager.BattleState.SELECTENEMY;

        Debug.Log("Entering enemy selection mode");
    }

    /// <summary>
    /// Enable/disable action buttons
    /// </summary>
    public void SetActionButtonsActive(bool active)
    {
        if (actionButtonPanel != null)
        {
            actionButtonPanel.SetButtonsInteractable(active);
        }
    }

    /// <summary>
    /// Force refresh semua UI
    /// </summary>
    [ContextMenu("Force Refresh All UI")]
    public void ForceRefreshAllUI()
    {
        foreach (var panel in characterPanels)
        {
            if (panel != null)
            {
                panel.UpdateUI();
            }
        }

        foreach (var panel in enemyPanels)
        {
            if (panel != null)
            {
                panel.UpdateUI();
            }
        }

        Debug.Log("All UI panels refreshed!");
    }
}