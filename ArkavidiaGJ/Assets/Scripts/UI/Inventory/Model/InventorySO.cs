using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

[CreateAssetMenu]
public class InventorySO : ScriptableObject
{
    [SerializeField] List<InventoryItems> _items;

    [field: SerializeField] public int Size { get; private set; }

    public event Action<Dictionary<int, InventoryItems>> OnInventoryChanged;

    public void InitializeInventory()
    {
        _items = new List<InventoryItems>();

        for (int i = 0; i < Size; i++)
        {
            _items.Add(InventoryItems.GetEmptyItem());
        }
    }

    public int AddItem(Item item, int quantity)
    {
        if (item.IsStackAble == false)
        {
            for (int i = 0; i < _items.Count; i++)
            {   
                if (IsInventoryFull())
                {
                    return quantity;                
                }

                while(quantity > 0 && IsInventoryFull() == false)
                {
                    quantity -= AddNonStackableItem(item, 1);
                }

                InformChange();
                return quantity;
                
            }
        }
        quantity = AddStackableItem(item, quantity);
        InformChange();
        return quantity;
    }

    private bool IsInventoryFull() => _items.Where(item => item._isEmpty).Any() == false;

    private int AddNonStackableItem(Item item, int quantity)
    {
        InventoryItems newItem = new InventoryItems
        {
            _item = item,
            _quantity = quantity
        };

        for (int i = 0; i <= _items.Count; i++)
        {
            if (_items[i]._isEmpty)
            {
                _items[i] = newItem;
                return quantity;
            }
        }

        return 0;
    }

    private int AddStackableItem(Item item, int quantity)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i]._isEmpty)
            {
                continue;
            }

            if (_items[i]._item.ID == item.ID)
            {
                int maxAmount = _items[i]._item.MaxStackSize - _items[i]._quantity;
                
                if (quantity > maxAmount)
                {
                    _items[i] = _items[i].ChangeQuantity(_items[i]._item.MaxStackSize);
                    quantity -= maxAmount;
                }
                else
                {
                    _items[i] = _items[i].ChangeQuantity(_items[i]._quantity + quantity);
                    InformChange();
                    return 0;
                }
            }
        }

        while (quantity > 0 && IsInventoryFull() == false)
        {
            int newQuantity = Mathf.Clamp(quantity, 0, item.MaxStackSize);
            quantity -= newQuantity;
            AddNonStackableItem(item, newQuantity);
        }
        return quantity;
    }

    public Dictionary<int, InventoryItems> GetCurrentInventoryState()
    {
        Dictionary<int, InventoryItems> returnValue = new Dictionary<int, InventoryItems>();

        for (int i = 0; i < _items.Count; i++)
        {
            if (_items[i]._isEmpty)
            {
                continue;
            }

            returnValue[i] = _items[i];
        }
        return returnValue;
    }

    public InventoryItems GetItemAt(int obj)
    {
        return _items[obj];
    }

    public void SwapItem(int arg1, int arg2)
    {
        InventoryItems firstItem = _items[arg1];
        _items[arg1] = _items[arg2];
        _items[arg2] = firstItem;
        InformChange();
    }

    private void InformChange()
    {
        OnInventoryChanged?.Invoke(GetCurrentInventoryState());
    }

    public void RemoveItem(int obj, int amount)
    {
        if (_items.Count > obj)
        {
            if (_items[obj]._isEmpty)
            {
                return;
            }

            int reminder = _items[obj]._quantity - amount;

            if (reminder <= 0)
            {
                _items[obj] = InventoryItems.GetEmptyItem();
            }
            else
            {
                _items[obj] = _items[obj].ChangeQuantity(reminder);
            }

            InformChange();
        }
    }
}

[Serializable]

public struct InventoryItems
{
    public int _quantity;
    public Item _item;
    public bool _isEmpty => _item == null;

    public InventoryItems ChangeQuantity(int newQuantity)
    {
        return new InventoryItems
        {
            _item = this._item,
            _quantity = newQuantity
        };
    }

    public static InventoryItems GetEmptyItem() => new InventoryItems()
    {
        _item = null,
        _quantity = 0
    };
};
