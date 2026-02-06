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

    // Quest tracking - persists karena DontDestroyOnLoad
    private HashSet<int> _completedQuestIndices = new HashSet<int>();
    private int _currentEnemyQuestIndex = -1;
    private bool _lastBattleWon = false;

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

    public void SetCurrentEnemyQuestIndex(int questIndex)
    {
        _currentEnemyQuestIndex = questIndex;
    }

    public void StartBattle(List<Characters> enemies, string returnScene)
    {
        PlayerStatus[] players = FindObjectsByType<PlayerStatus>(FindObjectsSortMode.None);
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

    public void OnBattleWon()
    {
        _lastBattleWon = true;
        if (_currentEnemyQuestIndex >= 0)
        {
            _completedQuestIndices.Add(_currentEnemyQuestIndex);
            Debug.Log($"Quest {_currentEnemyQuestIndex} completed! Enemy defeated.");
        }
    }

    public void OnBattleLost()
    {
        _lastBattleWon = false;
    }

    public bool IsQuestCompleted(int questIndex)
    {
        return _completedQuestIndices.Contains(questIndex);
    }

    public int GetCompletedQuestCount()
    {
        return _completedQuestIndices.Count;
    }

    // Cek dan reset hasil battle terakhir (dipanggil QuestManager saat scene load)
    public bool CheckAndClearBattleResult()
    {
        if (_lastBattleWon)
        {
            _lastBattleWon = false;
            return true;
        }
        return false;
    }

    public void ReturnToWorld()
    {
        if (_currentSetup != null)
        {
            SceneManager.LoadScene(_currentSetup._returnScene);
        }
    }
}
