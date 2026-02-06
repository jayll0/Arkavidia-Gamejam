using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Script untuk manage UI status satu enemy
/// Attach ke setiap panel Enemy1, Enemy2, Enemy3
/// </summary>
public class EnemyStatusUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyBattleMovement enemy; // Reference ke enemy yang akan di-track

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image healthBar;

    [Header("Text Labels (Optional)")]
    [SerializeField] private TextMeshProUGUI healthText; // Opsional: untuk tampilkan "100/100"

    [Header("Visual Feedback")]
    [SerializeField] private GameObject targetIndicator; // Opsional: arrow/glow saat enemy ini di-target
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color targetedColor = Color.yellow;
    [SerializeField] private Image panelBackground; // Opsional: untuk highlight panel
    [SerializeField] private GameObject panelRoot; // Reference ke panel utama (untuk hide/show)

    private void Start()
    {
        // Hide target indicator di awal
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(false);
        }

        // Initial update
        UpdateUI();
    }

    private void Update()
    {
        // Update UI setiap frame
        UpdateUI();

        // Check if this enemy is being targeted
        CheckTargetStatus();
    }

    /// <summary>
    /// Update semua UI element berdasarkan stats enemy
    /// </summary>
    public void UpdateUI()
    {
        // Jika enemy sudah mati atau null, hide panel
        if (enemy == null)
        {
            HidePanel();
            return;
        }

        if (enemy.hp <= 0)
        {
            HidePanel();
            return;
        }

        // Show panel jika enemy masih hidup
        ShowPanel();

        // Update name
        if (nameText != null)
        {
            nameText.text = enemy.gameObject.name;
        }

        // Update Health Bar
        if (healthBar != null)
        {
            float healthPercent = enemy.hp / enemy.maxHp;
            healthBar.fillAmount = healthPercent;
        }

        // Update Health Text
        if (healthText != null)
        {
            healthText.text = $"{(int)enemy.hp}/{(int)enemy.maxHp}";
        }
    }

    /// <summary>
    /// Cek apakah enemy ini sedang di-target
    /// </summary>
    private void CheckTargetStatus()
    {
        if (enemy == null) return;

        // Cek apakah enemy ini adalah selected target
        bool isTargeted = (BattleManager.selectedTarget == enemy);

        SetTargeted(isTargeted);
    }

    /// <summary>
    /// Set visual indicator untuk target
    /// </summary>
    private void SetTargeted(bool isTargeted)
    {
        // Show/hide target indicator
        if (targetIndicator != null)
        {
            targetIndicator.SetActive(isTargeted);
        }

        // Highlight panel background
        if (panelBackground != null)
        {
            panelBackground.color = isTargeted ? targetedColor : normalColor;
        }
    }

    /// <summary>
    /// Hide panel saat enemy mati
    /// </summary>
    private void HidePanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(false);
        }
        else
        {
            // Jika panelRoot tidak di-set, hide gameObject ini
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show panel saat enemy masih hidup
    /// </summary>
    private void ShowPanel()
    {
        if (panelRoot != null)
        {
            panelRoot.SetActive(true);
        }
        else
        {
            gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Method untuk set enemy reference dari script lain
    /// </summary>
    public void SetEnemy(EnemyBattleMovement newEnemy)
    {
        enemy = newEnemy;
        UpdateUI();
    }
}