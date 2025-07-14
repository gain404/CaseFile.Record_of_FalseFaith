using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    public NPCData npcData;
    
    public DialogueAsset GetFirstDialogue()
    {
        return npcData?.firstDialogueAsset;
    }

    public DialogueAsset GetSecondDialogue()
    {
        return npcData?.secondDialogueAsset;
    }
    
    public void OnInteract()
    {
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null) player.CurrentInteractableNPC = this;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var player = other.GetComponent<Player>();
            if (player != null && player.CurrentInteractableNPC == this) player.CurrentInteractableNPC = null;
        }
    }

    public string GetInteractPrompt()
    {
        return "F - 대화하기";
    }
}