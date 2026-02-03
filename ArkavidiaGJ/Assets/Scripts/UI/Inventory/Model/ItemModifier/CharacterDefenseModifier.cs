using UnityEngine;

[CreateAssetMenu]
public class CharacterDefenseModifier : CharacterModifier
{
    public override void AffectCharacter(GameObject character, int value)
    {
        PlayerStatus _status = character.GetComponent<PlayerStatus>();

        if (_status != null && _status._currentCharacter != null)
        {
            int newDefense = _status._currentCharacter.Defense + value;
            _status.UpdateDefense(newDefense);
        }
    }
}
