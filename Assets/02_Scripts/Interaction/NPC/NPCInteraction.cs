using UnityEngine;

public class NPCInteraction : MonoBehaviour, IInteractable
{
    public NPCData npcData;
    
    private bool hasStartedDialogue = false;
    void Start()
    {
        // Start 시 플레이어와 겹쳐있다면 바로 대화 시작
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                if (npcData == null || npcData.dialogueAsset == null)
                {
                    Debug.LogError("NPCData 또는 dialogueAsset이 null입니다.");
                    return;
                }

                DialogueManager.Instance.StartDialogue(npcData.dialogueAsset);
                hasStartedDialogue = true;
                break;
            }
        }
    }

    public string GetInteractPrompt()
    {
        return "F - 대화하기";  // 추후 텍스트 커스터마이징 가능
    }

    public void OnInteract()
    {
        DialogueManager.Instance.StartDialogue(npcData.dialogueAsset);
    }
}