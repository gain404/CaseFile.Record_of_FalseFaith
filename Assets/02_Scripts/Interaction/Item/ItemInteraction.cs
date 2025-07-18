using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    public ItemDialogueData dialogueData;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null)
            {
                player.CurrentInteractableItem = this;
                // 필요한 경우: "E - 조사하기" UI 표시
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null && player.CurrentInteractableItem == this)
            {
                player.CurrentInteractableItem = null;
                // 필요한 경우: 조사 UI 비활성화
            }
        }
    }


    public string GetInteractPrompt()
    {
        return "F - 조사하기"; //NPC와 동일
    }

    public void OnInteract()
    {
        if (dialogueData != null)
        {
            UIManager.Instance.UIDialogue.StartItemDialogue(dialogueData.dialogueLines, this.transform);
        }
    }
}