using Inventory;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject _map;
    [SerializeField] private GameObject _pause;

    [SerializeField] private InventoryController _inventoryController;
    [SerializeField] private GameObject _inventory;
    [SerializeField] private GameObject _backButton;

    public void PauseGame()
    {
        _pause.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OpenMap()
    {
        _map.SetActive(true);
        Time.timeScale = 0f;
    }

    public void OpenInventory()
    {
        _backButton.SetActive(true);
        _inventory.SetActive(true);
        _inventoryController.ShowInventory();
    }

    public void BackToMainScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
