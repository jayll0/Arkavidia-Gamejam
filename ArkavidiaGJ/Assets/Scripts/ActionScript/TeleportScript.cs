using UnityEngine;

public class TeleportScript : MonoBehaviour
{

    [SerializeField] private Collider2D _collider;
    [SerializeField] private GameObject _map;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _map.SetActive(true);
            Time.timeScale = 0f;
        }
    }

}
