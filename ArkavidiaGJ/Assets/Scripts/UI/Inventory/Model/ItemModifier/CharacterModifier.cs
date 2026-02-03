using UnityEngine;

[CreateAssetMenu(fileName = "CharacterModifier", menuName = "Scriptable Objects/CharacterModifier")]
public abstract class CharacterModifier : ScriptableObject
{
    public abstract void AffectCharacter(GameObject character, int value);
}
