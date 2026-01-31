using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Scene Name
    [SerializeField] private string _sceneName;
    [SerializeField] private GameObject _settingCanvas;

    // Functional Button
    public void goToScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    public void ActivateCanvas()
    {
        _settingCanvas.SetActive(true);
    }

    // Exit
    public void Exit()
    {
        Application.Quit();
    }
}
