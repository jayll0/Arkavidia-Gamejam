using UnityEngine;
using TMPro;

public class QuestManager : MonoBehaviour
{
    public static QuestManager Instance;

    [Header("UI Components")]
    public TextMeshProUGUI questText;

    [Header("Quest Data")]
    [TextArea(2, 5)]
    public string[] questList;

    private int currentQuestIndex = 0;

    private void Awake()
    {
        // Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Sinkronisasi quest index dengan enemy yang sudah dikalahkan
        if (EnemyScript.instance != null)
        {
            // Cek apakah baru menang battle
            EnemyScript.instance.CheckAndClearBattleResult();

            // Hitung quest index berdasarkan enemy yang sudah dikalahkan
            int completedCount = EnemyScript.instance.GetCompletedQuestCount();
            if (completedCount > currentQuestIndex)
            {
                currentQuestIndex = completedCount;
            }
        }

        UpdateQuestDisplay();
    }

    // Set teks quest secara manual
    public void SetQuest(string textBaru)
    {
        if (questText != null)
            questText.text = textBaru;
    }

    // Complete quest by index
    public void CompleteQuest(int questIndex)
    {
        if (EnemyScript.instance != null)
        {
            EnemyScript.instance.SetCurrentEnemyQuestIndex(questIndex);
            EnemyScript.instance.OnBattleWon();
        }

        if (questIndex >= currentQuestIndex)
        {
            currentQuestIndex = questIndex + 1;
        }

        UpdateQuestDisplay();
    }

    // Lanjut ke quest berikutnya
    public void NextQuest()
    {
        currentQuestIndex++;
        UpdateQuestDisplay();
    }

    // Cek apakah quest sudah selesai
    public bool IsQuestCompleted(int questIndex)
    {
        if (EnemyScript.instance != null)
        {
            return EnemyScript.instance.IsQuestCompleted(questIndex);
        }
        return questIndex < currentQuestIndex;
    }

    // Ambil index quest saat ini
    public int GetCurrentQuestIndex()
    {
        return currentQuestIndex;
    }

    // Update tampilan quest text
    private void UpdateQuestDisplay()
    {
        if (questText == null) return;

        if (currentQuestIndex < questList.Length)
        {
            questText.text = questList[currentQuestIndex];
        }
        else
        {
            questText.text = "Semua quest selesai!";
        }
    }
}
