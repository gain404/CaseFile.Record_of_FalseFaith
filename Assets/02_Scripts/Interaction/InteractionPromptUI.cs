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

    [Header("위치 설정")]
    [Tooltip("상호작용 대상의 머리 위에 얼마나 높게 띄울지 설정")]
    public float yOffset = 1.0f;

    private Camera mainCamera;
    private IInteractable currentTarget;

    private void Start()
    {
        mainCamera = Camera.main;
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
        
        if (currentTarget != null && interactPanel.activeSelf)
        {
            // IInteractable을 구현한 스크립트는 MonoBehaviour이므로 캐스팅이 가능
            var targetTransform = (currentTarget as MonoBehaviour).transform;
            
            // 월드 좌표를 스크린 좌표로 변환하여 UI 위치를 설정
            Vector3 targetPosition = targetTransform.position + Vector3.up * yOffset;
            Vector2 screenPosition = mainCamera.WorldToScreenPoint(targetPosition);
            interactPanel.transform.position = screenPosition;
        }
    }
}