using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatUI : MonoBehaviour
{
    [SerializeField] private Image _characterIcon;
    [SerializeField] private TMP_Text _characterName;
    [SerializeField] private TMP_Text _healthText;
    [SerializeField] private TMP_Text _manaText;
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _defenseText;
    [SerializeField] private TMP_Text _speedText;

    private void Awake()
    {
        ResetUI();
    }

    private void ResetUI()
    {
        this._characterIcon.gameObject.SetActive(false);
        this._characterName.text = "";
        this._healthText.text = "";
        this._manaText.text = "";
        this._attackText.text = "";
        this._defenseText.text = "";
        this._speedText.text = "";
    }

    public void setStats(Sprite sprite, string characterName, int health, int mana, int attack, int defense, int speed)
    {
        this._characterIcon.gameObject.SetActive (true);
        this._characterIcon.sprite = sprite;
        this._characterName.text = characterName;
        this._healthText.text = "Health : " + health + " / 100";
        this._manaText.text = "Mana : " + mana + " / 100";
        this._attackText.text = "Attack : " + attack.ToString();
        this._defenseText.text = "Defense   : " + defense.ToString();
        this._speedText.text = "Speed   : " + speed.ToString();
    }
}
