using UnityEngine;

public class NPCInteraction : MonoBehaviour
{
    public NPCData npcData;

    public void TryInteract()
    {
        DialogueManager.Instance.StartDialogue(npcData.dialogueAsset);
    }
}