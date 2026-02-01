using UnityEngine;

public class Interaction : MonoBehaviour
{

    [SerializeField] private GameObject _ask;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _ask.SetActive(true);
        }
                
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            _ask.SetActive(false);
        }
    }
}
