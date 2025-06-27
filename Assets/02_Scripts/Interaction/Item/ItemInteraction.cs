using UnityEngine;

public class ItemInteraction : MonoBehaviour, IInteractable
{
    public ItemDialogueData dialogueData;

    public string GetInteractPrompt()
    {
        return "F - 조사하기";
    }

    public void OnInteract()
    {
        if (dialogueData != null)
        {
            DialogueManager.Instance.ShowItemDialogue(dialogueData.dialogueLines);
        }
    }
}