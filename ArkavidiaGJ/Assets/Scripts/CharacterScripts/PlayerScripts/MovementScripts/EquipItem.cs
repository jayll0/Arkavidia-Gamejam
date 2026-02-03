using System;
using UnityEngine;

public class EquipItem : MonoBehaviour
{
    [SerializeField] private Equipment _equipment;
    [SerializeField] private InventorySO _inventoryData;
    
    public void SetItem(Equipment item)
    {
        if (_equipment != null && _equipment == item)
        {
            RemoveModifier(_equipment);
            _equipment = null;
            return;
        }

        if (_equipment != null)
        {
            RemoveModifier(_equipment);
        }

        _equipment = item;

        if (_equipment != null)
        {
            ApplyModifier(_equipment);
        }
}

    private void ApplyModifier(Equipment equipment)
    {
        foreach (ModifierData modifier in equipment.ModifierData)
        {
            modifier._statModifier.AffectCharacter(gameObject, modifier.value);
        }
    }

    private void RemoveModifier(Equipment equipment)
    {
        foreach(ModifierData modifier in equipment.ModifierData)
        {
            modifier._statModifier.AffectCharacter(gameObject, -modifier.value);
        }
    }
}
