using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    public NPCData npcData;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.CurrentInteractableNPC = this;
                // 필요한 경우: E키 안내 UI 활성화 등
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null && player.CurrentInteractableNPC == this)
            {
                player.CurrentInteractableNPC = null;
                // 필요한 경우: E키 안내 UI 비활성화 등
            }
        }
    }

    public string GetInteractPrompt()
    {
        return "E - 대화하기";  // 필요시 사용. E라는 이미지가 뜨면서 대화할 수 있다는 존재라는 걸 알릴 수 있음
    }

    public void OnInteract()
    {
        if (npcData == null || npcData.dialogueAsset == null)
        {
            Debug.LogError("NPCData 또는 dialogueAsset이 null입니다.");
            return;
        }

        DialogueManager.Instance.StartDialogue(npcData.dialogueAsset);
    }
}