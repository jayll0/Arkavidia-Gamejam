using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPage : MonoBehaviour
{

    [SerializeField] public InventoryItem _item;
    [SerializeField] public RectTransform _contentPanel;
    [SerializeField] private CharacterStatUI _charStats;
    [SerializeField] private InventoryDescription _itemDescription;
    [SerializeField] private MouseFollower _mouseFollower;

    List<InventoryItem> _listItems = new List<InventoryItem>();
    List<CharacterStatUI> _listCharacters = new List<CharacterStatUI>();

    public event Action<int> OnDescriptionRequested, OnItemActionRequested, OnStartDragging;
    public event Action<int, int> OnSwapItems;

    private int _currentlyDragIndex = -1;

    private void Awake()
    {
        Hide();
        _mouseFollower.Toggle(false);
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
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
        ResetSelection();
    }

    private void ResetSelection()
    {
        _itemDescription.ResetDescription();
        DeselectAllItems();
    }

    private void DeselectAllItems()
    {
        foreach (InventoryItem item in _listItems)
        {
            item.Deselect();
        }
    }

    public void Hide() 
    {
        gameObject.SetActive(false);
        ResetDragItem();
    }

    private void ResetDragItem()
    {
        _mouseFollower.Toggle(false);
        _currentlyDragIndex = -1;

    }

    public void UpdateData(int itemIndex, Sprite itemImage, int quantity)
    {
        if (_listItems.Count > itemIndex)
        {
            _listItems[itemIndex].SetData(itemImage, quantity);
        }
    }

    private void HandleShowItemActions(InventoryItem item)
    {
        
    }

    private void HandleSwap(InventoryItem item)
    {
        int index = _listItems.IndexOf(item);

        if (index == -1)
        {
            return;
        }

        OnSwapItems.Invoke(_currentlyDragIndex, index);
    }

    private void HandleEndDrag(InventoryItem item)
    {
        _mouseFollower.Toggle(false);
    }

    private void HandleBeginDrag(InventoryItem item)
    {
        int index = _listItems.IndexOf(item);

        if (index == -1)
        {
            return;
        }

        _currentlyDragIndex = index;
        HandleItemSelection(item);
        OnStartDragging?.Invoke(index);
    }

    public void CreateDragItem(Sprite sprite, int quantity)
    {
        _mouseFollower.Toggle(true);
        _mouseFollower.SetData(sprite, quantity);
    }

    private void HandleItemSelection(InventoryItem item)
    {
        int index = _listItems.IndexOf(item);

        if (index == -1)
        {
            return;
        }

        OnDescriptionRequested?.Invoke(index);
    }
}
