using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDescription : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;

    public void Awake()
    {
        ResetDescription();
    }

    public void ResetDescription()
    {
        this._image.gameObject.SetActive(false);
        this._title.text = "";
        this._description.text = "";
    }

    public void SetDescription(Sprite sprite, string name, string description)
    {
        this._image.gameObject.SetActive(true);
        this._image.sprite = sprite;
        this._title.text = name;
        this._description.text = description;
    }
}
