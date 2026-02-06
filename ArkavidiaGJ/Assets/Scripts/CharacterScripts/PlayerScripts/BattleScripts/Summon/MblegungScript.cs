using Unity.VisualScripting;
using UnityEngine;

public class MblegungScript : MonoBehaviour
{

    [SerializeField] private Animator _animator;

    void Start()
    {
        _animator = GetComponent<Animator>();

        var col = GetComponent<Collider2D>();

        Debug.Log($"Collider Trigger: {col.isTrigger}");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Kena Musuh");
        Destroy(gameObject, 1);
    }
}
