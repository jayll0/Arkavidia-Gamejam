using UnityEngine;
using TMPro; // Untuk TextMeshPro

public class DamageText : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float floatSpeed = 1f;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float lifetime = 1.5f;
    [SerializeField] private Vector3 floatDirection = new Vector3(0, 1, 0);

    [Header("Auto Fix Sorting (If text is behind sprites)")]
    [SerializeField] private bool autoFixSorting = true;
    [SerializeField] private string sortingLayerName = "UI";
    [SerializeField] private int sortingOrder = 999;
    [SerializeField] private float zPosition = -5f; // Negative = in front

    private TextMeshPro tmpText;
    private TextMesh legacyText;
    private Color originalColor;
    private float timer = 0f;
    private Vector3 startPosition;

    private void Awake()
    {
        // Try TextMeshPro first (recommended)
        tmpText = GetComponent<TextMeshPro>();

        // Fallback to legacy TextMesh
        if (tmpText == null)
        {
            legacyText = GetComponent<TextMesh>();
        }

        // AUTO FIX SORTING - Pastikan text di depan sprites
        if (autoFixSorting)
        {
            ApplySortingFix();
        }

        startPosition = transform.position;

        // Get original color
        if (tmpText != null)
        {
            originalColor = tmpText.color;
        }
        else if (legacyText != null)
        {
            originalColor = legacyText.color;
        }
    }

    private void ApplySortingFix()
    {
        // Fix untuk TextMeshPro
        if (tmpText != null)
        {
            tmpText.renderer.sortingLayerName = sortingLayerName;
            tmpText.sortingOrder = sortingOrder;
            Debug.Log($"DamageText: Set TMP sorting to {sortingLayerName} with order {sortingOrder}");
        }

        // Fix untuk Legacy TextMesh
        MeshRenderer mr = GetComponent<MeshRenderer>();
        if (mr != null)
        {
            mr.sortingLayerName = sortingLayerName;
            mr.sortingOrder = sortingOrder;
            Debug.Log($"DamageText: Set MeshRenderer sorting to {sortingLayerName} with order {sortingOrder}");
        }

        // Fix Z position (pastikan di depan)
        Vector3 pos = transform.position;
        pos.z = zPosition;
        transform.position = pos;
        startPosition = pos;
    }

    private void Update()
    {
        // Float up animation
        transform.position = startPosition + floatDirection * (floatSpeed * timer);

        // Fade out
        timer += Time.deltaTime;
        float alpha = Mathf.Lerp(1f, 0f, timer / lifetime);

        if (tmpText != null)
        {
            Color c = tmpText.color;
            c.a = alpha;
            tmpText.color = c;
        }
        else if (legacyText != null)
        {
            Color c = legacyText.color;
            c.a = alpha;
            legacyText.color = c;
        }
    }

    public void SetDamage(int damageValue)
    {
        if (tmpText != null)
        {
            tmpText.text = damageValue.ToString();
            Debug.Log($"DamageText spawned: {damageValue} at position {transform.position}");
        }
        else if (legacyText != null)
        {
            legacyText.text = damageValue.ToString();
            Debug.Log($"DamageText spawned: {damageValue} at position {transform.position}");
        }
        else
        {
            Debug.LogError("DamageText: No TextMeshPro or TextMesh component found!");
        }

        Destroy(gameObject, lifetime);
    }

    // Overload untuk critical hit, healing, dll
    public void SetDamage(int damageValue, Color color)
    {
        SetDamage(damageValue);

        if (tmpText != null)
        {
            tmpText.color = color;
            originalColor = color;
        }
        else if (legacyText != null)
        {
            legacyText.color = color;
            originalColor = color;
        }
    }

    public void SetText(string text)
    {
        if (tmpText != null)
        {
            tmpText.text = text;
        }
        else if (legacyText != null)
        {
            legacyText.text = text;
        }

        Destroy(gameObject, lifetime);
    }

    // Force apply sorting fix (untuk debugging)
    [ContextMenu("Force Apply Sorting Fix")]
    public void ForceApplySortingFix()
    {
        ApplySortingFix();
        Debug.Log("Sorting fix applied manually");
    }
}