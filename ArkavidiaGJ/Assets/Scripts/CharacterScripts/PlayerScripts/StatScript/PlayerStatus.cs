using UnityEditor.U2D.Animation;
using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] private Characters _character;

    public Characters _currentCharacter => _character;

    public void SetCharacter(Characters character)
    {
        _character = character;
    }

    public void UpdateHealth(int newHealth)
    {
        _character.Health = newHealth;
    }

    public void UpdateMana(int newMana)
    {
        _character.Mana = newMana;
    }

    public void UpdateAttack(int newAttack)
    {
        _character.Attack = newAttack;
    }

    public void UpdateDefense(int newDefense)
    {
        if (_character != null)
        {
            _character.Defense = newDefense;
        }
    }

    public void UpdateSpeed(int newSpeed)
    {
        if (_character != null)
        {
            _character.Speed = newSpeed;
        }
    }
}
