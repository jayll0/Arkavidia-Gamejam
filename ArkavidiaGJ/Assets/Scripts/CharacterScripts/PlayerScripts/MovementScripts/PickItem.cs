using GDS.Core.Events;
using System.Collections;
using UnityEngine;

public class PickItem : MonoBehaviour
{
    [field: SerializeField] public Item _item { get; private set; }
    [field: SerializeField] public int _quantity { get; set; } = 1;

    [SerializeField] AudioSource _audioSource;
    [SerializeField] float _duration = 0.3f;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = _item.Image;
    }

    public void DestroyItem()
    {
        GetComponent<Collider2D>().enabled = false;
        StartCoroutine(AnimateItemPickup());
    }

    private IEnumerator AnimateItemPickup()
    {
        _audioSource.Play();
        Vector3 startScale = transform.localScale;
        Vector3 endScale = Vector3.zero;

        float currentTime = 0;

        while (currentTime < _duration)
        {
            currentTime += Time.deltaTime;
            transform.localScale = Vector3.Lerp(startScale, endScale, currentTime / _duration);
            yield return null;
        }
        Destroy(gameObject);
    }
}
