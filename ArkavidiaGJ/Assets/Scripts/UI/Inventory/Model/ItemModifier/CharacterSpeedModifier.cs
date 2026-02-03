using UnityEngine;

[CreateAssetMenu]
public class CharacterSpeedModifier : CharacterModifier
{
    public override void AffectCharacter(GameObject character, int value)
    {
        PlayerStatus _status = character.GetComponent<PlayerStatus>();

        if (_status != null && _status._currentCharacter != null)
        {
            int newSpeed = _status._currentCharacter.Speed + value;
            _status.UpdateSpeed(newSpeed);
        }
    }
}
