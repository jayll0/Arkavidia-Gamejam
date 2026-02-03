using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] private int maxHealth = 10;
    [SerializeField] private Characters character;

    [SerializeField] private Renderer renderer;
    [SerializeField] private float flashTime = 0.2f;

    private void Start()
    {
        character.Health = 1;
    }

    public void Reduce(int damage)
    {
        character.Health -= damage / maxHealth;
        CreateHitFeedback();
        if (character.Health <= 0)
        {
            Die();
        }
    }

    public void AddHealth(int healthBoost)
    {
        int health = Mathf.RoundToInt(character.Health * maxHealth);
        int val = health + healthBoost;
        character.Health = (val > maxHealth ? maxHealth : val / maxHealth);
    }

    private void CreateHitFeedback()
    {
        StartCoroutine(FlashFeedback());
    }

    private IEnumerator FlashFeedback()
    {
        renderer.material.SetInt("_Flash", 1);
        yield return new WaitForSeconds(flashTime);
        renderer.material.SetInt("_Flash", 0);
    }

    private void Die()
    {
        Debug.Log("Died");
        character.Health = 1;
    }
}

