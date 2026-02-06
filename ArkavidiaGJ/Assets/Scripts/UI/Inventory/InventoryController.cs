using Inventory.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory
{
    public class InventoryController : MonoBehaviour
    {

        [SerializeField] private InventoryPage _inventoryUI;
        [SerializeField] private InventorySO _inventoryData;
        [SerializeField] private PlayerStatus _playerStatus;

        public List<InventoryItems> _itemInitiation = new List<InventoryItems>();

        public int size = 10;

        private bool isInBattle = false;

        private void Start()
        {
            isInBattle = SceneManager.GetActiveScene().name == "BattleScene";

            if (!isInBattle)
            {
                PrepareUI();
            }

            PrepareInventoryData();

            if (_playerStatus == null)
            {
                _playerStatus = FindObjectOfType<PlayerStatus>();
            }
        }

        public void Update()
        {
            if (!isInBattle && Input.GetKeyUp(KeyCode.I))
            {
                ToggleInventory();
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

        public void ToggleInventory()
        {
            if (_inventoryUI.isActiveAndEnabled == false)
            {
                ShowInventory();
            } 
            else
            {
                HideInventory();
            }
        }

        public void ShowInventory()
        {
            _inventoryUI.Show();
            Time.timeScale = 0f;

            _inventoryUI.ShowCharacterStatus(_playerStatus._currentCharacter);

            foreach (var item in _inventoryData.GetCurrentInventoryState())
            {
                _inventoryUI.UpdateData(item.Key, item.Value._item.Image, item.Value._quantity);
            }
        }

        public void HideInventory()
        {
            _inventoryUI.Hide();
            Time.timeScale = 1f;
        }
    }
}