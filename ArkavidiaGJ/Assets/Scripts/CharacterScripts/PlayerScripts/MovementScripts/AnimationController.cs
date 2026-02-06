using System;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnimationController : MonoBehaviour
{
    private Animator animator;
    private Action onAnimationComplete;

    void Start()
    {
        animator = GetComponent<Animator>();
        SceneCheck();
    }

    public void HandleAnimationAndFlip(float x, float y)
    {
        bool isMoving = (x != 0 || y != 0);

        if (isMoving)
        {
            animator.SetBool("IsMoving", true);
            animator.SetFloat("MoveX", x * 0.5f);
            animator.SetFloat("MoveY", y * 0.5f);
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }

    public void HandleAttackAnimation()
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttack", true);
    }

    public void HandleDeadAnimation()
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsHit", false);
        animator.SetBool("IsDead", true);
    }

    public void OnAnimationComplete()
    {
        onAnimationComplete?.Invoke();
        onAnimationComplete = null;
    }

    public void PlayAnimation(string AnimationName)
    {
        animator.SetTrigger(AnimationName);
    }

    public void PlayForcedAnimation(string AnimationName, Action callback)
    {
        this.onAnimationComplete = callback;
        PlayAnimation(AnimationName);
    }

    public void OnAttackComplete()
    {
        animator.SetBool("IsAttack", false);
        onAnimationComplete?.Invoke();
        onAnimationComplete = null;
    }

    public void HandleHitAnimation()
    {
        animator.SetBool("IsMoving", false);
        animator.SetBool("IsAttack", false);
        animator.SetBool("IsHit", true);
        animator.SetBool("IsDead", false);
    }

    public void OnHitComplete()
    {
        animator.SetBool("IsHit", false);
        onAnimationComplete?.Invoke();
        onAnimationComplete = null;
    }

    private void SceneCheck()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        bool isBattle = (currentScene == "Battle" || currentScene.ToLower().Contains("battle"));
        animator.SetBool("InBattle", isBattle);
    }
}