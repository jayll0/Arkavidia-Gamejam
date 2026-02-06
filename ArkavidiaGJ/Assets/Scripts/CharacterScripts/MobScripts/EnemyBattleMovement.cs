using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBattleMovement : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private Characters characters;

    [Header("UI")]
    public Transform damageTextPrefab;

    [Header("Visual Feedback")]
    [SerializeField] private GameObject selectionIndicator; // Opsional: arrow atau icon
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isHighlighted = false;

    private Vector3 originalPosition;
    private float attackDelay = 2.5f;
    public float hp, maxHp, sp, maxSp, atk, def;

    private void Start()
    {
        _animationController = GetComponent<AnimationController>();
        originalPosition = transform.position;

        // Inisialisasi stats
        hp = characters.Health;
        sp = characters.Speed;
        maxHp = characters.MaxHealth;
        maxSp = characters.MaxSpeed;

        atk = characters.Attack;
        def = characters.Defense;

        // Untuk visual feedback
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }

        // Hide selection indicator jika ada
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(false);
        }
    }

    private void Update()
    {
        // Cek kematian
        if (hp < 1)
        {
            Dead();
        }
    }

    /// <summary>
    /// Method untuk set highlight enemy (dipanggil dari BattleManager)
    /// </summary>
    public void SetHighlight(bool highlight, Color color)
    {
        isHighlighted = highlight;

        if (spriteRenderer != null)
        {
            if (highlight)
            {
                // Highlight dengan warna yang diberikan
                spriteRenderer.color = color;
            }
            else
            {
                // Kembalikan ke warna original
                spriteRenderer.color = originalColor;
            }
        }

        // Aktifkan/nonaktifkan selection indicator jika ada
        if (selectionIndicator != null)
        {
            selectionIndicator.SetActive(highlight);
        }
    }

    public void PerformMeleeAttack(Transform playerPosition)
    {
        StartCoroutine(AttackSequence(playerPosition));
    }

    public void ReturnToPosition()
    {
        transform.position = originalPosition;
    }

    public void TakeDamage(int damage)
    {
        hp -= damage;
        if (hp < 0) hp = 0;
        _animationController.HandleHitAnimation();
        Debug.Log($"{characters.Name} takes {damage} damage. HP: {hp}/{maxHp}");
    }

    private void Dead()
    {
        Debug.Log($"{characters.Name} has been defeated!");
        Destroy(gameObject);
    }

    private IEnumerator AttackSequence(Transform playerPosition)
    {
        // Untuk enemy melee
        if (characters.Name != "Dorothy")
        {
            BattleManager.currentDamage = characters.Attack;

            // Pindah ke posisi attack
            transform.position = new Vector3(playerPosition.position.x + 2.5f, playerPosition.position.y);
            _animationController.HandleAttackAnimation();

            yield return new WaitForSeconds(attackDelay);

            // Kembali ke posisi awal
            ReturnToPosition();
        }

        // Spawn damage text
        BattleManager.currentDamage = characters.Attack;
        if (damageTextPrefab != null)
        {
            Transform damTextObj = Instantiate(damageTextPrefab, playerPosition.position, Quaternion.identity);
            damTextObj.position = new Vector3(playerPosition.position.x, playerPosition.position.y + 1.5f, 0);

            DamageText dt = damTextObj.GetComponent<DamageText>();
            if (dt != null)
            {
                dt.SetDamage(characters.Attack);
            }
        }

        // Apply damage ke player
        BattleMovement player = playerPosition.GetComponent<BattleMovement>();
        if (player != null)
        {
            if (characters.Attack > player.def)
            {
                float totalDamage = (characters.Attack - player.def);
                player.TakeDamage((int)totalDamage);
                Debug.Log($"{characters.Name} deals {totalDamage} damage to {player.name}. Player HP: {player.hp}/{player.maxHp}");
            }
            else
            {
                Debug.Log($"{characters.Name}'s attack was blocked by {player.name}'s defense!");
            }
        }

        // Lanjut ke turn berikutnya
        BattleManager.Instance.NextTurn();
    }
}