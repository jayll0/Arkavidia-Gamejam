using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class EnemyScript : MonoBehaviour
{
    public static EnemyScript instance;

    [System.Serializable]
    public class BattleSetup
    {
        public List<Characters> _team;
        public List<Characters> _enemy;
        public string _returnScene;

    }

    public BattleSetup _currentSetup;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartBattle(List<Characters> enemies, string returnScene)
    {
        PlayerStatus[] players = FindObjectsOfType<PlayerStatus>();
        _currentSetup = new BattleSetup();
        _currentSetup._team = new List<Characters>();

        foreach (var player in players)
        {
            if (player._currentCharacter != null)
            {
                _currentSetup._team.Add(player._currentCharacter);
            }
        }

        _currentSetup._enemy = enemies;
        _currentSetup._returnScene = returnScene;

        SceneManager.LoadScene("Battle");
    }

    public void ReturnToWorld()
    {
        if (_currentSetup != null)
        {
            SceneManager.LoadScene(_currentSetup._returnScene);
        }
    }
}
