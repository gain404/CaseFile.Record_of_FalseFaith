using UnityEngine;
using TMPro;

public class InteractionPromptUI : MonoBehaviour
{
    [Header("필수 연결 요소")]
    [Tooltip("플레이어 게임 오브젝트를 연결")]
    public Player player;
    [Tooltip("팝업 UI의 부모 패널을 연결")]
    public GameObject interactPanel;
    [Tooltip("팝업에 표시될 TextMeshPro 오브젝트를 연결")]
    public TMP_Text promptText;
    
    private IInteractable currentTarget;

    private void Start()
    {
        interactPanel.SetActive(false);
    }

    private void Update()
    {
        IInteractable newTarget = null;
        if (player.CurrentInteractableNPC != null)
        {
            newTarget = player.CurrentInteractableNPC;
        }
        else if (player.CurrentInteractableItem != null)
        {
            newTarget = player.CurrentInteractableItem;
        }
        else if (player.itemData != null)
        {
            newTarget = new ItemDataInteractable(player.itemData);
        }
        
        if (newTarget != currentTarget)
        {
            currentTarget = newTarget;
            interactPanel.SetActive(currentTarget != null);

            if (currentTarget != null)
            {
                // 타겟의 종류에 맞는 프롬프트 텍스트를 가져와 UI에 표시
                promptText.text = currentTarget.GetInteractPrompt();
            }
        }
    }
}