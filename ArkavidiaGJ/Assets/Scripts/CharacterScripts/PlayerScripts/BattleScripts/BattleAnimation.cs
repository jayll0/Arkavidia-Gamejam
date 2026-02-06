using UnityEngine;
using System.Collections;

public class BattleAnimation : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Start()
    {
        _animator = GetComponent<Animator>();

        _animator.SetBool("InBattle", true);
    }

    public void Attack()
    {
        _animator.SetBool("IsAttack", true);
        StartCoroutine(ResetAfterDelay(0.5f));
    }

    public void Idle()
    {
        _animator.SetBool("IsAttack", false);
        _animator.SetBool("IsHit", false);
        _animator.SetBool("IsDead", false);

    }

    public void GotHit()
    {
        _animator.SetBool("IsHit", true);
        StartCoroutine(FlashRed());
        StartCoroutine(ResetAfterDelay(0.3f));
    }

    public void Dead()
    {
        _animator.SetBool("IsDead", true);
    }

    public void DoneBattle()
    {
        Idle();
        _animator.SetBool("InBattle", false);
    }

    System.Collections.IEnumerator FlashRed()
    {
        if (_spriteRenderer != null)
        {
            Color original = _spriteRenderer.color;
            _spriteRenderer.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            _spriteRenderer.color = original;
        }
    }

    IEnumerator ResetAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Idle();
    }
}
