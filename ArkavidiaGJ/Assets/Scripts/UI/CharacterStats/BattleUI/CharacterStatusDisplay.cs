using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script untuk manage UI status satu character (player)
/// Attach ke setiap panel Character1, Character2, Character3
/// </summary>
public class CharacterStatusUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BattleMovement character; // Reference ke character yang akan di-track

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image healthBar;
    [SerializeField] private Image manaBar;
    [SerializeField] private Image speedBar;

    [Header("Text Labels (Optional)")]
    [SerializeField] private TextMeshProUGUI healthText; // Opsional: untuk tampilkan "100/100"
    [SerializeField] private TextMeshProUGUI manaText;
    [SerializeField] private TextMeshProUGUI speedText;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject turnIndicator; // Opsional: arrow/glow saat giliran char ini
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color myTurnColor = Color.yellow;
    [SerializeField] private Image panelBackground; // Opsional: untuk highlight panel

    private void Start()
    {
        // Hide turn indicator di awal
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(false);
        }

        // Initial update
        UpdateUI();
    }

    private void Update()
    {
        // Update UI setiap frame (bisa dioptimasi dengan event)
        UpdateUI();

        // Check if it's this character's turn
        CheckTurnStatus();
    }

    /// <summary>
    /// Update semua UI element berdasarkan stats character
    /// </summary>
    public void UpdateUI()
    {
        if (character == null) return;

        // Update name
        if (nameText != null)
        {
            nameText.text = character.gameObject.name;
        }

        // Update Health Bar
        if (healthBar != null)
        {
            float healthPercent = character.hp / character.maxHp;
            healthBar.fillAmount = healthPercent;
        }

        // Update Health Text
        if (healthText != null)
        {
            healthText.text = $"{(int)character.hp}/{(int)character.maxHp}";
        }

        // Update Mana Bar
        if (manaBar != null)
        {
            float manaPercent = character.mp / character.maxMp;
            manaBar.fillAmount = manaPercent;
        }

        // Update Mana Text
        if (manaText != null)
        {
            manaText.text = $"{(int)character.mp}/{(int)character.maxMp}";
        }

        // Update Speed Bar
        if (speedBar != null)
        {
            float speedPercent = character.sp / character.maxSp;
            speedBar.fillAmount = speedPercent;
        }

        // Update Speed Text
        if (speedText != null)
        {
            speedText.text = $"{(int)character.sp}/{(int)character.maxSp}";
        }
    }

    /// <summary>
    /// Cek apakah sekarang giliran character ini
    /// </summary>
    private void CheckTurnStatus()
    {
        if (BattleManager.Instance == null || character == null) return;

        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();

        if (currentEntity != null && currentEntity.isPlayer && currentEntity.playerMovement == character)
        {
            // Ini turn character ini!
            SetTurnActive(true);
        }
        else
        {
            // Bukan turn character ini
            SetTurnActive(false);
        }
    }

    /// <summary>
    /// Set visual indicator untuk turn
    /// </summary>
    private void SetTurnActive(bool isActive)
    {
        // Show/hide turn indicator
        if (turnIndicator != null)
        {
            turnIndicator.SetActive(isActive);
        }

        // Highlight panel background
        if (panelBackground != null)
        {
            panelBackground.color = isActive ? myTurnColor : normalColor;
        }
    }

    /// <summary>
    /// Method untuk set character reference dari script lain
    /// </summary>
    public void SetCharacter(BattleMovement newCharacter)
    {
        character = newCharacter;
        UpdateUI();
    }
}