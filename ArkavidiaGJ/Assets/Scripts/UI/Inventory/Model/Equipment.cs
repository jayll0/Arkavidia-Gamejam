using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Equipment", menuName = "Scriptable Objects/Equipment")]
public class Equipment : Item, IItemAction
{
    [SerializeField] private List<ModifierData> _modifierData = new List<ModifierData>();
    public List<ModifierData> ModifierData => _modifierData;

    public string ActionName => "Equip";
    public AudioClip ActionSFX { get; private set; }

    public bool PerformAction(GameObject character)
    {
        EquipItem equipment = character.GetComponent<EquipItem>();

        if (equipment != null)
        {
            equipment.SetItem(this);
            return true;
        }
        return false;
    }
}