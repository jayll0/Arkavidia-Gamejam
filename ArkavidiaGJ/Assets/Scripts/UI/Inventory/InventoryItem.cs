using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    [SerializeField] Image _itemImage;
    [SerializeField] Image _border;
    [SerializeField] TMP_Text _quantity;

    public event Action<InventoryItem> OnItemClicked, OnItemDropped, OnItemBeginDrag, OnItemEndDrag, OnItemRightMouseButtonClick, OnItemHoverEnter, OnItemHoverExit;

    private bool empty = true;

    public void Awake()
    {
        ResetData();
        Deselect();
    }

    public void Deselect()
    {
        _border.enabled = false;
    }

    public void Select()
    {
        _border.enabled = true;
    }

    public void ResetData()
    {
        this._itemImage.gameObject.SetActive(false);
        empty = true;
    }

    public void SetData(Sprite sprite, int quantity)
    {
        this._itemImage.gameObject.SetActive(true);
        this._itemImage.sprite = sprite;
        this._quantity.text = quantity + "";
        empty = false;
    }

    public void OnBeginDrag()
    {
        if (empty)
        {
            return;
        }

        OnItemBeginDrag?.Invoke(this);
    }

    public void OnEndDrag()
    {
        OnItemEndDrag?.Invoke(this);
    }
    
    public void OnDrop()
    {
        OnItemDropped?.Invoke(this);
    }

    public void OnPointerClick(BaseEventData data)
    {
        PointerEventData pointerData = (PointerEventData)data;

        if (pointerData.button == PointerEventData.InputButton.Right)
        {
            OnItemRightMouseButtonClick?.Invoke(this);
        }
        else
        {
            OnItemClicked?.Invoke(this);
        }
    }
    
    public void OnPointerEnter()
    {
        if (empty)
        {
            return;
        }
        OnItemHoverEnter?.Invoke(this);
    }

    public void OnPointerExit()
    {
        OnItemHoverExit?.Invoke(this);
    }
}
