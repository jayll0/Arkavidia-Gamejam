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
        // Tampilkan quest pertama saat game mulai
        if (questList.Length > 0)
        {
            SetQuest(questList[currentQuestIndex]);
        }
    }

    // Set teks quest secara manual
    public void SetQuest(string textBaru)
    {
        questText.text = textBaru;
    }

    // Lanjut ke quest berikutnya
    public void NextQuest()
    {
        currentQuestIndex++;

        if (currentQuestIndex < questList.Length)
        {
            SetQuest(questList[currentQuestIndex]);
        }
        else
        {
            questText.text = "Semua quest selesai!";
        }
    }

    // Ambil index quest saat ini (opsional)
    public int GetCurrentQuestIndex()
    {
        return currentQuestIndex;
    }
}
