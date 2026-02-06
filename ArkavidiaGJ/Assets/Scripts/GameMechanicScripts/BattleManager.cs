using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    [Header("Battle Participants - Manual Assignment (Optional)")]
    [Tooltip("Kosongkan jika ingin auto-detect, atau isi manual di Inspector")]
    public List<BattleMovement> playerCharacters = new List<BattleMovement>();
    public List<EnemyBattleMovement> enemyCharacters = new List<EnemyBattleMovement>();

    [Header("Turn System Settings")]
    [SerializeField] private bool autoDetectCharacters = true;
    [SerializeField] private float battleStartDelay = 0.5f;

    [Header("Target Selection")]
    [SerializeField] private int currentTargetIndex = 0; // Index enemy yang dipilih
    [SerializeField] private Color highlightColor = Color.yellow;
    [SerializeField] private Color normalColor = Color.white;

    [Header("Debug Info")]
    [SerializeField] private int currentTurnIndex = 0;
    [SerializeField] private List<BattleEntity> turnOrder = new List<BattleEntity>();

    public static int currentDamage = 0;
    public static event Action<int> OnTurnChanged;

    // Untuk sistem target selection
    public static EnemyBattleMovement selectedTarget = null;
    public static bool waitingForTargetSelection = false;

    private bool battleInitialized = false;
    private List<EnemyBattleMovement> aliveEnemies = new List<EnemyBattleMovement>();


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
        Invoke(nameof(InitializeBattle), battleStartDelay);
    }

    private void Update()
    {
        // Handle target selection dengan arrow keys
        if (waitingForTargetSelection)
        {
            HandleTargetSelection();
        }
    }

    private void HandleTargetSelection()
    {
        // Update list enemy yang masih hidup
        UpdateAliveEnemiesList();

        if (aliveEnemies.Count == 0) return;

        // Arrow Right - Next target
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            currentTargetIndex++;
            if (currentTargetIndex >= aliveEnemies.Count)
            {
                currentTargetIndex = 0; // Loop ke awal
            }
            UpdateTargetHighlight();
            Debug.Log($"Target: {aliveEnemies[currentTargetIndex].gameObject.name} ({currentTargetIndex + 1}/{aliveEnemies.Count})");
        }

        // Arrow Left - Previous target
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            currentTargetIndex--;
            if (currentTargetIndex < 0)
            {
                currentTargetIndex = aliveEnemies.Count - 1; // Loop ke akhir
            }
            UpdateTargetHighlight();
            Debug.Log($"Target: {aliveEnemies[currentTargetIndex].gameObject.name} ({currentTargetIndex + 1}/{aliveEnemies.Count})");
        }

        // Enter/Space - Confirm target dan attack
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmTarget();
        }
    }

    private void UpdateAliveEnemiesList()
    {
        aliveEnemies = enemyCharacters.Where(e => e != null && e.hp > 0).ToList();

        // Pastikan currentTargetIndex valid
        if (currentTargetIndex >= aliveEnemies.Count)
        {
            currentTargetIndex = aliveEnemies.Count - 1;
        }
        if (currentTargetIndex < 0 && aliveEnemies.Count > 0)
        {
            currentTargetIndex = 0;
        }
    }

    private void UpdateTargetHighlight()
    {
        // Reset semua enemy ke warna normal
        foreach (var enemy in aliveEnemies)
        {
            enemy.SetHighlight(false, normalColor);
        }

        // Highlight enemy yang dipilih
        if (currentTargetIndex >= 0 && currentTargetIndex < aliveEnemies.Count)
        {
            aliveEnemies[currentTargetIndex].SetHighlight(true, highlightColor);
            selectedTarget = aliveEnemies[currentTargetIndex];
        }
    }

    private void ConfirmTarget()
    {
        if (currentTargetIndex >= 0 && currentTargetIndex < aliveEnemies.Count)
        {
            EnemyBattleMovement target = aliveEnemies[currentTargetIndex];

            var currentEntity = GetCurrentTurnEntity();
            if (currentEntity != null && currentEntity.isPlayer)
            {
                Debug.Log($"<color=green>Attack confirmed! {currentEntity.characterName} → {target.gameObject.name}</color>");
                PlayerAttackTarget(currentEntity.playerMovement, target);

                // Reset highlights
                ClearAllHighlights();
            }
        }
    }

    private void ClearAllHighlights()
    {
        foreach (var enemy in enemyCharacters)
        {
            if (enemy != null)
            {
                enemy.SetHighlight(false, normalColor);
            }
        }
    }

    private void InitializeBattle()
    {
        if (battleInitialized) return;

        Debug.Log("=== Initializing Battle ===");

        if (autoDetectCharacters || (playerCharacters.Count == 0 && enemyCharacters.Count == 0))
        {
            DetectCharacters();
        }

        Debug.Log($"Found {playerCharacters.Count} players and {enemyCharacters.Count} enemies");

        CreateTurnOrder();

        if (turnOrder.Count > 0)
        {
            battleInitialized = true;
            Debug.Log("Battle initialized successfully!");
            StartNextTurn();
        }
        else
        {
            Debug.LogError("Failed to initialize battle: No participants found!");
            Debug.LogError("Make sure your player/enemy GameObjects have BattleMovement/EnemyBattleMovement components!");
        }
    }

    private void DetectCharacters()
    {
        playerCharacters.Clear();
        enemyCharacters.Clear();

        BattleMovement[] foundPlayers = FindObjectsOfType<BattleMovement>();
        foreach (var player in foundPlayers)
        {
            playerCharacters.Add(player);
            Debug.Log($"Detected Player: {player.gameObject.name}");
        }

        EnemyBattleMovement[] foundEnemies = FindObjectsOfType<EnemyBattleMovement>();
        foreach (var enemy in foundEnemies)
        {
            enemyCharacters.Add(enemy);
            Debug.Log($"Detected Enemy: {enemy.gameObject.name}");
        }

        if (foundPlayers.Length == 0 && foundEnemies.Length == 0)
        {
            Debug.LogWarning("No characters detected! Make sure:");
            Debug.LogWarning("1. Characters have BattleMovement or EnemyBattleMovement component");
            Debug.LogWarning("2. Characters are active in the scene");
            Debug.LogWarning("3. Scripts are properly attached");
        }
    }

    private void CreateTurnOrder()
    {
        turnOrder.Clear();

        foreach (var player in playerCharacters)
        {
            if (player != null && player.hp > 0)
            {
                turnOrder.Add(new BattleEntity
                {
                    isPlayer = true,
                    playerMovement = player,
                    enemyMovement = null,
                    speed = player.sp,
                    characterName = player.gameObject.name
                });
            }
        }

        foreach (var enemy in enemyCharacters)
        {
            if (enemy != null && enemy.hp > 0)
            {
                turnOrder.Add(new BattleEntity
                {
                    isPlayer = false,
                    playerMovement = null,
                    enemyMovement = enemy,
                    speed = enemy.sp,
                    characterName = enemy.gameObject.name
                });
            }
        }

        turnOrder = turnOrder.OrderByDescending(e => e.speed).ToList();

        Debug.Log($"Turn order created with {turnOrder.Count} participants");

        for (int i = 0; i < turnOrder.Count; i++)
        {
            Debug.Log($"Turn {i + 1}: {turnOrder[i].characterName} (Speed: {turnOrder[i].speed})");
        }
    }

    private void StartNextTurn()
    {
        if (!battleInitialized) return;

        RefreshTurnOrder();

        if (turnOrder.Count == 0)
        {
            Debug.Log("Battle ended - no participants left");
            return;
        }

        if (CheckBattleEnd())
        {
            return;
        }

        BattleEntity current = turnOrder[currentTurnIndex];

        if (current.isPlayer)
        {
            // Set flag untuk menunggu player memilih target
            waitingForTargetSelection = true;
            selectedTarget = null;
            currentTargetIndex = 0; // Reset ke target pertama

            UpdateAliveEnemiesList();
            UpdateTargetHighlight(); // Highlight target pertama

            Debug.Log($">>> PLAYER TURN: {current.characterName} <<<");
            Debug.Log("Use ← → Arrow Keys to select target, SPACE/ENTER to attack!");

            OnTurnChanged?.Invoke(currentTurnIndex);
        }
        else
        {
            waitingForTargetSelection = false;
            ClearAllHighlights();

            Debug.Log($">>> ENEMY TURN: {current.characterName} (Auto-attacking...) <<<");
            OnTurnChanged?.Invoke(currentTurnIndex);
            Invoke(nameof(ExecuteEnemyTurn), 0.5f);
        }
    }

    private void ExecuteEnemyTurn()
    {
        if (currentTurnIndex < turnOrder.Count)
        {
            BattleEntity current = turnOrder[currentTurnIndex];
            if (!current.isPlayer && current.enemyMovement != null)
            {
                EnemyAutoAttack(current.enemyMovement);
            }
        }
    }

    private void EnemyAutoAttack(EnemyBattleMovement enemy)
    {
        List<BattleMovement> alivePlayers = playerCharacters.Where(p => p != null && p.hp > 0).ToList();

        if (alivePlayers.Count == 0)
        {
            Debug.Log("No alive players to attack");
            NextTurn();
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, alivePlayers.Count);
        BattleMovement target = alivePlayers[randomIndex];

        Debug.Log($"{enemy.gameObject.name} attacks {target.gameObject.name}");
        enemy.PerformMeleeAttack(target.transform);
    }

    public void PlayerAttackTarget(BattleMovement player, EnemyBattleMovement target)
    {
        if (target == null || target.hp <= 0)
        {
            Debug.Log("Invalid target!");
            return;
        }

        Debug.Log($"{player.gameObject.name} attacks {target.gameObject.name}");
        player.PerformMeleeAttack(target.transform);

        // Reset selection
        waitingForTargetSelection = false;
        selectedTarget = null;
    }

    public void PlayerAttack(BattleMovement player)
    {
        // Jika ada target yang dipilih, serang target tersebut
        if (selectedTarget != null)
        {
            PlayerAttackTarget(player, selectedTarget);
            return;
        }

        // Jika tidak ada target, pilih random (fallback)
        List<EnemyBattleMovement> aliveEnemies = enemyCharacters.Where(e => e != null && e.hp > 0).ToList();

        if (aliveEnemies.Count == 0)
        {
            Debug.Log("No alive enemies to attack");
            NextTurn();
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, aliveEnemies.Count);
        EnemyBattleMovement target = aliveEnemies[randomIndex];

        Debug.Log($"{player.gameObject.name} attacks {target.gameObject.name}");
        player.PerformMeleeAttack(target.transform);
    }

    public void NextTurn()
    {
        currentTurnIndex++;

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
            Debug.Log("=== NEW ROUND ===");
            RefreshTurnOrder();
        }

        StartNextTurn();
    }

    private void RefreshTurnOrder()
    {
        int beforeCount = turnOrder.Count;

        turnOrder.RemoveAll(e =>
            (e.isPlayer && (e.playerMovement == null || e.playerMovement.hp <= 0)) ||
            (!e.isPlayer && (e.enemyMovement == null || e.enemyMovement.hp <= 0))
        );

        int afterCount = turnOrder.Count;
        if (beforeCount != afterCount)
        {
            Debug.Log($"Turn order updated: {beforeCount} -> {afterCount} participants");
        }

        if (currentTurnIndex >= turnOrder.Count && turnOrder.Count > 0)
        {
            currentTurnIndex = 0;
        }
    }

    private bool CheckBattleEnd()
    {
        bool anyPlayerAlive = playerCharacters.Any(p => p != null && p.hp > 0);
        bool anyEnemyAlive = enemyCharacters.Any(e => e != null && e.hp > 0);

        if (!anyPlayerAlive)
        {
            Debug.Log("==============================");
            Debug.Log("       DEFEAT! YOU LOST       ");
            Debug.Log("==============================");
            return true;
        }

        if (!anyEnemyAlive)
        {
            Debug.Log("==============================");
            Debug.Log("      VICTORY! YOU WIN!       ");
            Debug.Log("==============================");
            return true;
        }

        return false;
    }

    public BattleEntity GetCurrentTurnEntity()
    {
        if (battleInitialized && currentTurnIndex < turnOrder.Count)
        {
            return turnOrder[currentTurnIndex];
        }
        return null;
    }

    [ContextMenu("Re-Initialize Battle")]
    public void ReInitializeBattle()
    {
        battleInitialized = false;
        currentTurnIndex = 0;
        turnOrder.Clear();
        InitializeBattle();
    }

    [ContextMenu("Force Detect Characters")]
    public void ForceDetectCharacters()
    {
        DetectCharacters();
        Debug.Log($"Manual detection: {playerCharacters.Count} players, {enemyCharacters.Count} enemies");
    }

    [System.Serializable]
    public class BattleEntity
    {
        public bool isPlayer;
        public BattleMovement playerMovement;
        public EnemyBattleMovement enemyMovement;
        public float speed;
        public string characterName;
    }
}