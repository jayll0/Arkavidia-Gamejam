using UnityEngine;
using System.Collections.Generic;

public class InventoryController : MonoBehaviour
{

    [SerializeField] private InventoryPage _inventoryUI;

    public int size = 10;

    private void Start()
    {
        _inventoryUI.InitializeInventoryUI(size);
    }

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            if (_inventoryUI.isActiveAndEnabled == false)
            {
                _inventoryUI.Show();
            }
            else
            {
                _inventoryUI.Hide();
            }
        }
    }
}
