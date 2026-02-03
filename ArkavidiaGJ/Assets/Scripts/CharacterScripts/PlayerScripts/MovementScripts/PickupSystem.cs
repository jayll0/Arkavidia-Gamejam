using UnityEngine;

public class PickupSystem : MonoBehaviour
{
    [SerializeField] InventorySO _inventoryData;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PickItem item = collision.GetComponent<PickItem>();

        if (item != null)
        {
            int reminder = _inventoryData.AddItem(item._item, item._quantity);

            if (reminder == 0)
            {
                item.DestroyItem();
            }
            else
            {
                item._quantity = reminder;
            }
        }
    }
}
