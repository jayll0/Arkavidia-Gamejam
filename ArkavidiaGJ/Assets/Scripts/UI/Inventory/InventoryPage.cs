using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

namespace Inventory.UI
{
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

        public void ShowCharacterStatus(Characters chara)
        {
            if (chara != null)
            {
                _charStats.setStats(
                    chara.Image,
                    chara.Name,
                    chara.Health,
                    chara.Mana,
                    chara.Attack,
                    chara.Defense,
                    chara.Speed
                );
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            ResetSelection();
        }

        public void ResetSelection()
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
            int index = _listItems.IndexOf(item);

            if (index == -1)
            {
                return;
            }

            OnItemActionRequested?.Invoke(index);
        }

        private void HandleSwap(InventoryItem item)
        {
            int index = _listItems.IndexOf(item);

            if (index == -1)
            {
                return;
            }

            OnSwapItems.Invoke(_currentlyDragIndex, index);
            HandleItemSelection(item);
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

        internal void UpdateDescription(int obj, Sprite image, string name, string description)
        {
            _itemDescription.SetDescription(image, name, description);
            DeselectAllItems();
            _listItems[obj].Select();
        }

        internal void ResetAllItems()
        {
            foreach (var item in _listItems)
            { 
                item.ResetData();
                item.Deselect();
            }
        }
    }
}