using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemActionPanel : MonoBehaviour
{
    [SerializeField] private GameObject _buttonPrefab;
    [SerializeField] private Transform _buttonContainer;

    public event Action<int> OnActionButtonClicked;

    public void ShowActions(string[] actions)
    {
        foreach (Transform child in _buttonContainer)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < actions.Length; i++)
        {
            int actionIndex = i;
            GameObject buttonObject = Instantiate(_buttonPrefab, _buttonContainer);

            Button button = buttonObject.GetComponent<Button>();
            TMP_Text text = buttonObject.GetComponentInChildren<TMP_Text>();

            if (text != null)
            {
                text.text = actions[i];
            }

            if (button != null)
            {
                button.onClick.AddListener(() => OnActionButtonClicked?.Invoke(actionIndex));
            }
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}