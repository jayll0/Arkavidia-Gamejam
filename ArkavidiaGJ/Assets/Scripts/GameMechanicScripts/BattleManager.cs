using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance { get; private set; }

    // Battle State Enum untuk kontrol flow battle
    public enum BattleState
    {
        WAITING,        // Menunggu player input
        SELECTENEMY,    // Player sedang pilih target untuk attack
        PROCESSING,     // Sedang proses attack/action
        ENEMYTURN,      // Giliran enemy
        BATTLEEND       // Battle selesai
    }

    [Header("Battle State")]
    public BattleState state = BattleState.WAITING;

    [Header("Battle Participants - Manual Assignment (Optional)")]
    [Tooltip("Kosongkan jika ingin auto-detect, atau isi manual di Inspector")]
    public List<BattleMovement> playerCharacters = new List<BattleMovement>();
    public List<EnemyBattleMovement> enemyCharacters = new List<EnemyBattleMovement>();

    [Header("Turn System Settings")]
    [SerializeField] private bool autoDetectCharacters = true;
    [SerializeField] private float battleStartDelay = 0.5f;
    [SerializeField] private float turnEndDelay = 3.0f; // ✅ Delay sebelum next turn

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
        // Handle target selection dengan arrow keys (keyboard fallback)
        if (state == BattleState.SELECTENEMY)
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

        // Enter/Space - Confirm target dan attack (keyboard fallback)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            ConfirmTargetKeyboard();
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

    private void ConfirmTargetKeyboard()
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
            state = BattleState.WAITING;
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
            state = BattleState.BATTLEEND;
            return;
        }

        if (CheckBattleEnd())
        {
            return;
        }

        BattleEntity current = turnOrder[currentTurnIndex];

        if (current.isPlayer)
        {
            // Player turn - set state ke WAITING agar button bisa diklik
            state = BattleState.WAITING;
            selectedTarget = null;
            currentTargetIndex = 0;

            Debug.Log($">>> PLAYER TURN: {current.characterName} <<<");
            Debug.Log("Waiting for player input... (Use buttons or arrow keys)");

            OnTurnChanged?.Invoke(currentTurnIndex);
        }
        else
        {
            // Enemy turn
            state = BattleState.ENEMYTURN;
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
            EndTurn();
            return;
        }

        int randomIndex = UnityEngine.Random.Range(0, alivePlayers.Count);
        BattleMovement target = alivePlayers[randomIndex];

        Debug.Log($"{enemy.gameObject.name} attacks {target.gameObject.name}");
        enemy.PerformMeleeAttack(target.transform);

        // ✅ End turn setelah attack selesai (dengan delay)
        Invoke(nameof(EndTurn), turnEndDelay);
    }

    public void PlayerAttackTarget(BattleMovement player, EnemyBattleMovement target)
    {
        if (target == null || target.hp <= 0)
        {
            Debug.Log("Invalid target!");
            state = BattleState.WAITING; // ✅ Kembali ke WAITING jika target invalid
            return;
        }

        // Set state ke PROCESSING
        state = BattleState.PROCESSING;

        Debug.Log($"{player.gameObject.name} attacks {target.gameObject.name}");
        player.PerformMeleeAttack(target.transform);

        // Reset selection
        selectedTarget = null;

        // ✅ End turn setelah attack selesai (dengan delay)
        Invoke(nameof(EndTurn), turnEndDelay);
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
            EndTurn();
            return;
        }

        // ✅ Set state ke PROCESSING sebelum attack
        state = BattleState.PROCESSING;

        int randomIndex = UnityEngine.Random.Range(0, aliveEnemies.Count);
        EnemyBattleMovement target = aliveEnemies[randomIndex];

        Debug.Log($"{player.gameObject.name} attacks {target.gameObject.name}");
        player.PerformMeleeAttack(target.transform);

        // ✅ End turn setelah attack selesai (dengan delay)
        Invoke(nameof(EndTurn), turnEndDelay);
    }

    /// <summary>
    /// Method untuk end turn - dipanggil setelah action selesai
    /// </summary>
    public void EndTurn()
    {
        // Cek apakah battle sudah selesai sebelum lanjut ke turn berikutnya
        if (CheckBattleEnd()) return;

        NextTurn();
    }

    public void NextTurn()
    {
        currentTurnIndex++;

        // Refresh list untuk membuang karakter yang sudah mati/null
        RefreshTurnOrder();

        if (currentTurnIndex >= turnOrder.Count)
        {
            currentTurnIndex = 0;
            Debug.Log("=== NEW ROUND ===");
        }

        StartNextTurn();
    }

    private void RefreshTurnOrder()
    {
        // Menghapus entitas yang HP nya 0 atau object-nya sudah di-destroy
        turnOrder.RemoveAll(e =>
            (e.isPlayer && (e.playerMovement == null || e.playerMovement.hp <= 0)) ||
            (!e.isPlayer && (e.enemyMovement == null || e.enemyMovement.hp <= 0))
        );

        // Proteksi: Jika index out of range setelah penghapusan
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
            state = BattleState.BATTLEEND;
            Debug.Log("==============================");
            Debug.Log("       DEFEAT! YOU LOST       ");
            Debug.Log("==============================");
            return true;
        }

        if (!anyEnemyAlive)
        {
            state = BattleState.BATTLEEND;
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
        state = BattleState.WAITING;
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