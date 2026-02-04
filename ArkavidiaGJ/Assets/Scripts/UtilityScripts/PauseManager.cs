using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    [Header("Components")]
    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private GameObject _settingCanvas;
    [SerializeField] private string _scene;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void GoToScene()
    {
        SceneManager.LoadScene(_scene);
    }

    public void OpenSettings()
    {
        _settingCanvas.SetActive(true);
    }

    public void Resume()
    {
        _pauseCanvas.SetActive(false);
        Time.timeScale = 1f;
    }
}
