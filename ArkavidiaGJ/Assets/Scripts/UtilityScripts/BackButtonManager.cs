using UnityEngine;

public class BackButtonManager : MonoBehaviour
{
    [SerializeField] private GameObject _inventory;
    public void CloseInventory()
    {
        _inventory.SetActive(false);
    }
}
