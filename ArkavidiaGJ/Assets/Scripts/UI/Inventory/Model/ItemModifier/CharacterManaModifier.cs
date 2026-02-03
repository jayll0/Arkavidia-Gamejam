using UnityEngine;

[CreateAssetMenu]
public class CharacterManaModifier : CharacterModifier
{
    public override void AffectCharacter(GameObject character, int value)
    {
        PlayerStatus _status = character.GetComponent<PlayerStatus>();

        if (_status != null && _status._currentCharacter != null)
        {
            int newMana = _status._currentCharacter.Mana + value;

            if (newMana > 100)
            {
                newMana = 100;
            }

            _status.UpdateMana(newMana);
        }
    }
}
