using Inventory;
using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Script untuk manage tombol-tombol aksi di battle (Attack, Inventory, Flee)
/// Attach ke GameObject yang memiliki ketiga button tersebut
/// VERSI ROBUST - dengan full null safety checks
/// </summary>
public class ActionButtonPanel : MonoBehaviour
{
    [Header("Button References")]
    public Button _attackButton;
    public Button _inventoryButton;
    public Button _guardButton; // Bisa juga untuk Flee

    [Header("Settings")]
    [SerializeField] private bool _useGuardAsFlee = true; // Toggle antara Guard atau Flee
    [SerializeField] private bool _debugMode = true; // Enable untuk debug logs

    private BattleUIManager _battleUIManager;
    private BattlePanelManager _battlePanelManager;
    private InventoryController _inventoryController;
    private BattleMovement _currentPlayerCharacter;
    private bool _buttonsEnabled = true;

    private void Start()
    {
        if (_debugMode) Debug.Log("[ActionButtonPanel] Starting initialization...");

        // Setup button listeners
        SetupButtons();

        // Initial state - disable buttons until player's turn
        SetButtonsInteractable(false);

        if (_debugMode) Debug.Log("[ActionButtonPanel] Initialization complete");
    }

    private void Update()
    {
        // Check if it's player's turn and update button states
        UpdateButtonStates();

        // Find managers jika belum ditemukan (lazy loading)
        EnsureManagerReferences();
    }

    /// <summary>
    /// Ensure all manager references ada (lazy loading)
    /// </summary>
    private void EnsureManagerReferences()
    {
        if (_battleUIManager == null)
        {
            _battleUIManager = BattleUIManager.Instance;
        }

        if (_battlePanelManager == null)
        {
            _battlePanelManager = FindObjectOfType<BattlePanelManager>();
        }

        if (_inventoryController == null)
        {
            _inventoryController = FindObjectOfType<InventoryController>();
        }
    }

    /// <summary>
    /// Setup listeners untuk semua button
    /// </summary>
    private void SetupButtons()
    {
        if (_attackButton != null)
        {
            _attackButton.onClick.RemoveAllListeners(); // Clear existing listeners
            _attackButton.onClick.AddListener(OnAttackButtonClicked);
            if (_debugMode) Debug.Log("[ActionButtonPanel] Attack button listener added");
        }
        else
        {
            Debug.LogWarning("[ActionButtonPanel] Attack button reference is NULL! Assign it in Inspector.");
        }

        if (_inventoryButton != null)
        {
            _inventoryButton.onClick.RemoveAllListeners();
            _inventoryButton.onClick.AddListener(OnInventoryButtonClicked);
            if (_debugMode) Debug.Log("[ActionButtonPanel] Inventory button listener added");
        }
        else
        {
            Debug.LogWarning("[ActionButtonPanel] Inventory button reference is NULL! Assign it in Inspector.");
        }

        if (_guardButton != null)
        {
            _guardButton.onClick.RemoveAllListeners();
            if (_useGuardAsFlee)
            {
                _guardButton.onClick.AddListener(OnFleeButtonClicked);
                if (_debugMode) Debug.Log("[ActionButtonPanel] Flee button listener added");
            }
            else
            {
                _guardButton.onClick.AddListener(OnGuardButtonClicked);
                if (_debugMode) Debug.Log("[ActionButtonPanel] Guard button listener added");
            }
        }
        else
        {
            Debug.LogWarning("[ActionButtonPanel] Guard/Flee button reference is NULL! Assign it in Inspector.");
        }
    }

    /// <summary>
    /// Update state button berdasarkan kondisi battle
    /// </summary>
    private void UpdateButtonStates()
    {
        // Early return jika BattleManager belum ada
        if (BattleManager.Instance == null)
        {
            SetButtonsInteractable(false);
            return;
        }

        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();

        // Check if it's a player's turn
        bool isPlayerTurn = currentEntity != null && currentEntity.isPlayer;

        if (isPlayerTurn)
        {
            _currentPlayerCharacter = currentEntity.playerMovement;

            // Enable buttons saat player turn dan battle dalam state WAITING
            bool shouldEnable = BattleManager.Instance.state == BattleManager.BattleState.WAITING;
            SetButtonsInteractable(shouldEnable);
        }
        else
        {
            _currentPlayerCharacter = null;
            SetButtonsInteractable(false);
        }
    }

    /// <summary>
    /// Handler untuk Attack button
    /// </summary>
    private void OnAttackButtonClicked()
    {
        if (_debugMode) Debug.Log("[ActionButtonPanel] Attack button clicked!");

        // Comprehensive null checks
        if (!ValidateBattleManager()) return;
        if (!ValidatePlayerTurn()) return;
        if (!ValidateEnemiesExist()) return;

        // Enter attack mode - player needs to select target
        BattleManager.Instance.state = BattleManager.BattleState.SELECTENEMY;

        // Update battle text
        UpdateBattleText("Select an enemy to attack!");

        // Disable buttons while selecting
        SetButtonsInteractable(false);

        if (_debugMode) Debug.Log("[ActionButtonPanel] Entering SELECTENEMY mode");
    }

    /// <summary>
    /// Handler untuk Inventory button
    /// </summary>
    private void OnInventoryButtonClicked()
    {
        if (_debugMode) Debug.Log("[ActionButtonPanel] Inventory button clicked!");

        if (_inventoryController == null)
        {
            _inventoryController = FindObjectOfType<InventoryController>();

            if (_inventoryController == null)
            {
                Debug.LogError("[ActionButtonPanel] InventoryController not found in scene!");
                UpdateBattleText("Inventory system not available!");
                return;
            }
        }

        // Show inventory
        _inventoryController.ShowInventory();

        // Update battle text
        UpdateBattleText("Using item from inventory...");

        if (_debugMode) Debug.Log("[ActionButtonPanel] Inventory opened");
    }

    /// <summary>
    /// Handler untuk Guard button
    /// </summary>
    private void OnGuardButtonClicked()
    {
        if (_debugMode) Debug.Log("[ActionButtonPanel] Guard button clicked!");

        if (!ValidateBattleManager()) return;
        if (!ValidatePlayerTurn()) return;

        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();
        if (currentEntity == null || currentEntity.playerMovement == null)
        {
            Debug.LogError("[ActionButtonPanel] Current player character is null!");
            return;
        }

        // Implement guard logic
        UpdateBattleText($"{currentEntity.playerMovement.gameObject.name} is guarding!");

        // Set guard status (you might need to add this field to BattleMovement)
        // currentEntity.playerMovement.isGuarding = true;

        // End turn
        BattleManager.Instance.EndTurn();

        if (_debugMode) Debug.Log($"[ActionButtonPanel] {currentEntity.playerMovement.gameObject.name} is guarding");
    }

    /// <summary>
    /// Handler untuk Flee button
    /// </summary>
    private void OnFleeButtonClicked()
    {
        if (_debugMode) Debug.Log("[ActionButtonPanel] Flee button clicked!");

        if (!ValidateBattleManager()) return;
        if (!ValidatePlayerTurn()) return;

        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();
        if (currentEntity == null || currentEntity.playerMovement == null)
        {
            Debug.LogError("[ActionButtonPanel] Current player character is null!");
            return;
        }

        BattleMovement player = currentEntity.playerMovement;

        // Calculate flee success rate (50% base + speed bonus)
        float fleeChance = 0.5f;
        fleeChance += (player.sp / player.maxSp) * 0.3f;

        float randomValue = UnityEngine.Random.Range(0f, 1f);

        if (randomValue <= fleeChance)
        {
            // Flee success!
            UpdateBattleText("Successfully fled from battle!");

            if (_debugMode) Debug.Log($"[ActionButtonPanel] Flee successful! ({fleeChance * 100:F0}% chance)");

            // TODO: Implement scene change
            // UnityEngine.SceneManagement.SceneManager.LoadScene("PreviousScene");
        }
        else
        {
            // Flee failed - end turn
            UpdateBattleText("Failed to flee!");
            BattleManager.Instance.EndTurn();

            if (_debugMode) Debug.Log($"[ActionButtonPanel] Flee failed! ({fleeChance * 100:F0}% chance)");
        }
    }

    /// <summary>
    /// Validate BattleManager exists
    /// </summary>
    private bool ValidateBattleManager()
    {
        if (BattleManager.Instance == null)
        {
            Debug.LogError("[ActionButtonPanel] BattleManager.Instance is NULL! Make sure BattleManager exists in scene.");
            UpdateBattleText("Battle system error!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Validate it's player's turn
    /// </summary>
    private bool ValidatePlayerTurn()
    {
        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();

        if (currentEntity == null)
        {
            Debug.LogWarning("[ActionButtonPanel] No current turn entity!");
            UpdateBattleText("Wait for your turn!");
            return false;
        }

        if (!currentEntity.isPlayer)
        {
            Debug.LogWarning("[ActionButtonPanel] Not player's turn!");
            UpdateBattleText("Wait for your turn!");
            return false;
        }

        if (currentEntity.playerMovement == null)
        {
            Debug.LogError("[ActionButtonPanel] Current player movement is NULL!");
            return false;
        }

        return true;
    }

    /// <summary>
    /// Validate enemies exist
    /// </summary>
    private bool ValidateEnemiesExist()
    {
        if (BattleManager.Instance.enemyCharacters == null ||
            BattleManager.Instance.enemyCharacters.Count == 0)
        {
            Debug.LogWarning("[ActionButtonPanel] No enemies to attack!");
            UpdateBattleText("No enemies to attack!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Update battle text safely
    /// </summary>
    private void UpdateBattleText(string text)
    {
        if (_battlePanelManager != null && _battlePanelManager.battleText != null)
        {
            _battlePanelManager.battleText.text = text;
        }
        else
        {
            if (_debugMode) Debug.Log($"[ActionButtonPanel] Battle Text: {text}");
        }
    }

    /// <summary>
    /// Enable/disable semua buttons
    /// </summary>
    public void SetButtonsInteractable(bool interactable)
    {
        _buttonsEnabled = interactable;

        if (_attackButton != null)
        {
            _attackButton.interactable = interactable;
        }

        if (_inventoryButton != null)
        {
            _inventoryButton.interactable = interactable;
        }

        if (_guardButton != null)
        {
            _guardButton.interactable = interactable;
        }
    }

    /// <summary>
    /// Force show buttons (untuk testing)
    /// </summary>
    [ContextMenu("Force Enable Buttons")]
    public void ForceEnableButtons()
    {
        SetButtonsInteractable(true);
        if (_debugMode) Debug.Log("[ActionButtonPanel] Buttons force enabled");
    }

    /// <summary>
    /// Force hide buttons (untuk testing)
    /// </summary>
    [ContextMenu("Force Disable Buttons")]
    public void ForceDisableButtons()
    {
        SetButtonsInteractable(false);
        if (_debugMode) Debug.Log("[ActionButtonPanel] Buttons force disabled");
    }

    /// <summary>
    /// Validate setup (untuk testing di Inspector)
    /// </summary>
    [ContextMenu("Validate Setup")]
    private void ValidateSetup()
    {
        Debug.Log("=== ActionButtonPanel Validation ===");
        Debug.Log($"Attack Button: {(_attackButton != null ? "OK" : "MISSING")}");
        Debug.Log($"Inventory Button: {(_inventoryButton != null ? "OK" : "MISSING")}");
        Debug.Log($"Guard/Flee Button: {(_guardButton != null ? "OK" : "MISSING")}");
        Debug.Log($"BattleManager: {(BattleManager.Instance != null ? "OK" : "MISSING")}");
        Debug.Log($"BattlePanelManager: {(_battlePanelManager != null ? "OK" : "MISSING")}");
        Debug.Log($"InventoryController: {(_inventoryController != null ? "OK" : "MISSING")}");
        Debug.Log("=====================================");
    }
}