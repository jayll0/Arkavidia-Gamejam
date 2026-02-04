using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    [SerializeField] private string _mapName;
    [SerializeField] private GameObject _mapCanvas;

    // Masuk ke map tertentu
    public void GoToMap()
    {
        SceneManager.LoadScene(_mapName);
        Time.timeScale = 1.0f;
    }

    public void CloseMap()
    {
        _mapCanvas.SetActive(false);
        Time.timeScale = 1.0f;
    } 

}
