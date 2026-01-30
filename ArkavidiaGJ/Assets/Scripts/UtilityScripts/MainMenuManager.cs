using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    // Scene Name
    [SerializeField] private string _sceneName;

    // Functional Button
    public void goToScene()
    {
        SceneManager.LoadScene(_sceneName);
    }

    // Exit
    public void Exit()
    {
        Application.Quit();
    }
}
