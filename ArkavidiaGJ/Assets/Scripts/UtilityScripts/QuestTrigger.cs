using UnityEngine;

public class QuestTrigger : MonoBehaviour
{
    [TextArea] // Biar kolom isiannya lebar di Inspector
    public string pesanQuest; 

    private void OnTriggerEnter(Collider other)
    {
        // Pastikan Player kamu punya Tag "Player"
        if (other.CompareTag("Player"))
        {
            // Panggil QuestManager untuk ubah teks
            QuestManager.Instance.SetQuest(pesanQuest);
            
            // Matikan trigger ini supaya gak kepanggil ulang
            gameObject.SetActive(false); 
        }
    }
}