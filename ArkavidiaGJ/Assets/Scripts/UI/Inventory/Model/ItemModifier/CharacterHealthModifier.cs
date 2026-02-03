using UnityEngine;

[CreateAssetMenu(fileName = "CharacterHealthModifier", menuName = "Scriptable Objects/CharacterHealthModifier")]
public class CharacterHealthModifier : CharacterModifier
{
    public override void AffectCharacter(GameObject character, int value)
    {
        PlayerStatus _status = character.GetComponent<PlayerStatus>();

        if (_status != null && _status._currentCharacter != null)
        {
            int newHealth = _status._currentCharacter.Health + value;

            if (newHealth > 100)
            {
                newHealth = 100;
            }

            _status.UpdateHealth(newHealth);
        }
    }
}
