using UnityEngine;

[CreateAssetMenu]
public class CharacterAttackModifier : CharacterModifier
{
    public override void AffectCharacter(GameObject character, int value)
    {
        PlayerStatus _status = character.GetComponent<PlayerStatus>();

        if (_status != null && _status._currentCharacter != null)
        {
            int newAttack = _status._currentCharacter.Attack + value;
            _status.UpdateAttack(newAttack);
        }
    }
}
