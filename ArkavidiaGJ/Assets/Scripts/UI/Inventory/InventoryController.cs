using UnityEngine;
using System.Collections.Generic;
using System;
using Inventory.UI;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {

        [SerializeField] private InventoryPage _inventoryUI;
        [SerializeField] private InventorySO _inventoryData;
        [SerializeField] private PlayerStatus _playerStatus;

        public List<InventoryItems> _itemInitiation = new List<InventoryItems>();

        public int size = 10;

        private void Start()
        {
            PrepareUI();
            PrepareInventoryData();

            if (_playerStatus == null)
            {
                _playerStatus = FindObjectOfType<PlayerStatus>();
            }
        }

        private void PrepareInventoryData()
        {
            _inventoryData.InitializeInventory();
            _inventoryData.OnInventoryChanged += UpdateInventoryUI;

            foreach (InventoryItems item in _itemInitiation)
            {
                if (item._isEmpty)
                {
                    continue;
                }

                _inventoryData.AddItem(item._item, item._quantity);
            }
        }

        private void UpdateInventoryUI(Dictionary<int, InventoryItems> inventoryState)
        {
            _inventoryUI.ResetAllItems();

            foreach (var item in inventoryState) 
            { 
                _inventoryUI.UpdateData(item.Key, item.Value._item.Image, item.Value._quantity);
            }
        }

        private void PrepareUI()
        {
            _inventoryUI.InitializeInventoryUI(_inventoryData.Size);
            _inventoryUI.OnDescriptionRequested += HandleDescriptionRequest;
            _inventoryUI.OnSwapItems += HandleSwapItem;
            _inventoryUI.OnStartDragging += HandleDragging;
            _inventoryUI.OnItemActionRequested += HandleItemActionRequest;
        }

        private void HandleItemActionRequest(int obj)
        {
            InventoryItems inventoryItems = _inventoryData.GetItemAt(obj);

            if (inventoryItems._isEmpty)
            {
                return;
            }

            IItemAction itemAction = inventoryItems._item as IItemAction;

            if (itemAction != null)
            {
                if (_playerStatus != null)
                {
                    itemAction.PerformAction(_playerStatus.gameObject);

                    if (_playerStatus._currentCharacter != null)
                    {
                        _inventoryUI.ShowCharacterStatus(_playerStatus._currentCharacter);
                    }
                }
            }

            IDestroyableItem destroyableItem = inventoryItems._item as IDestroyableItem;

            if (destroyableItem != null)
            {
                _inventoryData.RemoveItem(obj, 1);
            }
        }

        private void HandleDragging(int obj)
        {
            InventoryItems inventoryItems = _inventoryData.GetItemAt(obj);

            if (inventoryItems._isEmpty)
            {
                return;
            }

            _inventoryUI.CreateDragItem(inventoryItems._item.Image, inventoryItems._quantity);
        }

        private void HandleSwapItem(int arg1, int arg2)
        {
            _inventoryData.SwapItem(arg1, arg2);
        }

        private void HandleDescriptionRequest(int obj)
        {
            InventoryItems inventoryItem = _inventoryData.GetItemAt(obj);

            if (inventoryItem._isEmpty)
            {
                _inventoryUI.ResetSelection();
                return;
            }

            Item item = inventoryItem._item;

            _inventoryUI.UpdateDescription(obj, item.Image, item.Name, item.Description);
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.I))
            {
                if (_inventoryUI.isActiveAndEnabled == false)
                {
                    _inventoryUI.Show();

                    _inventoryUI.ShowCharacterStatus(_playerStatus._currentCharacter);

                    foreach (var item in _inventoryData.GetCurrentInventoryState())
                    {
                        _inventoryUI.UpdateData(item.Key, item.Value._item.Image, item.Value._quantity);
                    }
                }
                else
                {
                    _inventoryUI.Hide();
                }
            }
        }
    }
}