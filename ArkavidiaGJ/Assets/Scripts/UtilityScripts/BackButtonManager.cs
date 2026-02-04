using UnityEngine;

public class BackButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _inventory;
    public void CloseInventory()
    {
        _inventory.SetActive(false);
        this.gameObject.SetActive(false);
        Time.timeScale = 1.0f;
    }
}
