using NUnit.Framework;
using System.Collections.Generic;
using System;
using UnityEngine;

[CreateAssetMenu]
public class Consumables : Item, IDestroyableItem, IItemAction
{
    [SerializeField] private List<ModifierData> _modifierData = new List<ModifierData>();
    public string ActionName => "Consume";

    public AudioClip ActionSFX {get; private set;}

    public bool PerformAction(GameObject character)
    {
       foreach (ModifierData modifierData in _modifierData)
        {
            modifierData._statModifier.AffectCharacter(character, modifierData.value);
        }

       return true;
    }
}

public interface IDestroyableItem
{

}

public interface IItemAction
{
    public string ActionName { get; }
    public AudioClip ActionSFX { get; }
    bool PerformAction(GameObject character);
}

[Serializable]
public class ModifierData
{
    public CharacterModifier _statModifier;
    public int value;
}