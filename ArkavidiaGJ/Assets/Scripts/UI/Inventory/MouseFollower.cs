using UnityEngine;

public class MouseFollower : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    [SerializeField] private InventoryItem _item;

    private void Awake()
    {
        _canvas = GetComponentInParent<Canvas>();
        _item = GetComponentInChildren<InventoryItem>();
    }

    public void SetData(Sprite image, int quantity)
    {
        _item.SetData(image, quantity);
    }

    private void Update()
    {
        Vector2 position;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)_canvas.transform,
            Input.mousePosition,
            _canvas.worldCamera,
            out position
            );

        transform.position = _canvas.transform.TransformPoint(position);
    }

    public void Toggle(bool value)
    {
        gameObject.SetActive(value);
    }
}
