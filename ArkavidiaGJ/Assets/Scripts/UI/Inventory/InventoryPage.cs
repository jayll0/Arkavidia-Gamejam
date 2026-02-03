using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{

    [SerializeField] public InventoryItem _item;
    [SerializeField] public RectTransform _contentPanel;
    [SerializeField] private CharacterStatUI _charStats;
    [SerializeField] private InventoryDescription _itemDescription;

    List<InventoryItem> _listItems = new List<InventoryItem>();

    public Sprite _characterIcon, _itemImage;
    public string _characterName, _itemName, _itemDesc;
    public int _quantity, _health, _mana, _attack, _defense, _speed;

    private void Awake()
    {
        Hide();
        _itemDescription.ResetDescription();
    }

    public void InitializeInventoryUI(int size)
    {
        for (int i = 0; i < size; i++)
        {
            InventoryItem item = Instantiate(_item, Vector3.zero, Quaternion.identity);
            item.transform.SetParent(_contentPanel);
            _listItems.Add(item);

            item.OnItemClicked += HandleItemSelection;
            item.OnItemBeginDrag += HandleBeginDrag;
            item.OnItemEndDrag += HandleEndDrag;
            item.OnItemDropped += HandleSwap;
            item.OnItemRightMouseButtonClick += HandleShowItemActions;
            item.OnItemHoverEnter += HandleItemHoverEnter;
            item.OnItemHoverExit += HandleItemHoverExit;
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        _itemDescription.ResetDescription();

        _charStats.setStats(_characterIcon, _characterName, _health, _mana, _attack, _defense, _speed);

        _listItems[0].SetData(_itemImage, _quantity);
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
    }

    private void HandleShowItemActions(InventoryItem item)
    {
        
    }

    private void HandleSwap(InventoryItem item)
    {
        
    }

    private void HandleEndDrag(InventoryItem item)
    {
        
    }

    private void HandleBeginDrag(InventoryItem item)
    {
        
    }

    private void HandleItemSelection(InventoryItem item)
    {
        _itemDescription.SetDescription(_itemImage, _itemName, _itemDesc);
    }

    private void HandleItemHoverEnter(InventoryItem item)
    {
        _itemDescription.SetDescription(_itemImage, _itemName, _itemDesc);
    }

    private void HandleItemHoverExit(InventoryItem item)
    {
        _itemDescription.ResetDescription();
    }
}
