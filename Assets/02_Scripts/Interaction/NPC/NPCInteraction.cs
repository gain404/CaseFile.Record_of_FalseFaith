using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCData npcData;

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 감지됨");

            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("F 키 눌림 → 대화 시작");

                if (DialogueManager.Instance == null)
                    Debug.LogError("DialogueManager.Instance is NULL");

                DialogueManager.Instance.StartDialogue(npcData.dialogueAsset);
            }
        }
    }
}