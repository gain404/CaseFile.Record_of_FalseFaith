using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    public NPCData npcData;
    private Player _player;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
    }
    
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

    public void OffInput()
    {
        _player.PlayerController.playerActions.Disable();
    }
    
    public void OnEnterDialogue()
    {
        _player.CurrentInteractableNPC = this;
        UIManager.Instance.UIDialogue.StartDialogue(GetFirstDialogue(), transform);
        UIManager.Instance.UIDialogue.autoAdvanced = true;
    }

    public void OnExitDialogue()
    {
        Debug.Log("ExitDialogue 호출");
        _player.PlayerController.playerActions.Enable();
        _player.CurrentInteractableNPC = null;
        UIManager.Instance.UIDialogue.autoAdvanced = false;
        gameObject.SetActive(false);
    }
    
}