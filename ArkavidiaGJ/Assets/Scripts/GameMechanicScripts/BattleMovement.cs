using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMovement : MonoBehaviour
{
    [Header("Component")]
    [SerializeField] private AnimationController _animationController;
    [SerializeField] private Characters characters;

    [Header("UI")]
    public Transform damageTextPrefab;

    private Vector3 originalPosition;
    private float attackDelay = 2.5f;
    public float hp, mp, maxHp, maxMp, sp, maxSp, atk, def;

    private bool isMyTurn = false;

    private void Start()
    {
        _animationController = GetComponent<AnimationController>();
        originalPosition = transform.position;

        // Inisialisasi stats
        hp = characters.Health;
        mp = characters.Mana;
        sp = characters.Speed;
        maxHp = characters.MaxHealth;
        maxMp = characters.MaxMana;
        maxSp = characters.MaxSpeed;

        atk = characters.Attack;
        def = characters.Defense;
    }

    private void Update()
    {
        // Cek apakah ini turn player ini
        CheckIfMyTurn();

        // Cek kematian
        if (hp < 1)
        {
            Dead();
        }
    }

    // Cek apakah sekarang turn karakter ini
    private void CheckIfMyTurn()
    {
        if (BattleManager.Instance == null) return;

        var currentEntity = BattleManager.Instance.GetCurrentTurnEntity();
        if (currentEntity != null && currentEntity.isPlayer && currentEntity.playerMovement == this)
        {
            isMyTurn = true;
            // Bisa tambahkan visual indicator (highlight, arrow, dll)
        }
        else
        {
            isMyTurn = false;
        }
    }

    public void PerformMeleeAttack(Transform enemyPosition)
    {
        StartCoroutine(AttackSequence(enemyPosition));
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
        _animationController.HandleDeadAnimation();
        Debug.Log($"{characters.Name} has died!");
    }

    private IEnumerator AttackSequence(Transform enemyPosition)
    {
        // Untuk karakter melee (bukan Dorothy)
        if (characters.Name != "Dorothy")
        {
            // Pindah ke posisi attack
            transform.position = new Vector3(enemyPosition.position.x - 2.5f, enemyPosition.position.y);
            _animationController.HandleAttackAnimation();
            BattleManager.currentDamage = characters.Attack;

            yield return new WaitForSeconds(attackDelay);

            // Kembali ke posisi awal
            ReturnToPosition();
        }
        // Untuk Dorothy (ranged attack)
        else
        {
            BattleManager.currentDamage = characters.Attack;
            GameObject projectile = Instantiate(characters.Object, transform.position, Quaternion.identity);
            _animationController.HandleAttackAnimation();
            projectile.transform.position = new Vector3(enemyPosition.position.x, enemyPosition.position.y);

            yield return new WaitForSeconds(attackDelay);
        }

        // Spawn damage text
        BattleManager.currentDamage = characters.Attack;
        if (damageTextPrefab != null)
        {
            Transform damTextObj = Instantiate(damageTextPrefab, enemyPosition.position, Quaternion.identity);
            damTextObj.position = new Vector3(enemyPosition.position.x, enemyPosition.position.y + 1.5f, 0);

            DamageText dt = damTextObj.GetComponent<DamageText>();
            if (dt != null)
            {
                dt.SetDamage(characters.Attack);
            }
        }

        // Apply damage ke enemy
        EnemyBattleMovement enemy = enemyPosition.GetComponent<EnemyBattleMovement>();
        if (enemy != null)
        {
            if (characters.Attack > enemy.def)
            {
                float totalDamage = (characters.Attack - enemy.def);
                enemy.TakeDamage((int)totalDamage);
                Debug.Log($"{characters.Name} deals {totalDamage} damage to {enemy.name}. Enemy HP: {enemy.hp}/{enemy.maxHp}");
            }
            else
            {
                Debug.Log($"{characters.Name}'s attack was blocked by {enemy.name}'s defense!");
            }
        }

        // Lanjut ke turn berikutnya
        BattleManager.Instance.NextTurn();
    }

    // Method untuk UI button attack (alternatif)
    public void OnAttackButtonPressed()
    {
        if (isMyTurn)
        {
            BattleManager.Instance.PlayerAttack(this);
        }
    }
}