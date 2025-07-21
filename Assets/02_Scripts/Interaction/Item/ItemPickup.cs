using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickup : MonoBehaviour
{
    /// <summary>
    /// csv용 아이템 배치 설정
    /// </summary>

    [Header("아이템 설정")]
    public int itemId;
    public int quantity = 1;

    [Header("상호작용 설정")]
    public float interactionRange = 2f;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    [Header("시각적 효과")]
    public bool enableRotation = true;
    public float rotationSpeed = 90f;
    public bool enableBobbing = true;
    public float bobbingHeight = 0.5f;
    public float bobbingSpeed = 2f;

    private bool playerInRange = false;
    private Vector2 startPosition;
    private ItemData itemData;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // 시작 위치 가져오기
        startPosition = transform.position;

        // 아이템 데이터 가져오기
        itemData = ItemDatabase.Instance.GetItemData(itemId);

        if (itemData == null)
        {
            Debug.LogError($"아이템 ID {itemId}를 찾을 수 없습니다!");
            return;
        }

        // 상호작용 UI 초기 비활성화
        if (interactionUI != null)
            interactionUI.SetActive(false);

        // 상호작용 텍스트 설정
        if (interactionText != null)
        {
            interactionText.text = $"Press F to pick up {itemData.itemName}";
        }

        // 아이템 이름으로 게임오브젝트 이름 설정
        gameObject.name = $"ItemPickup_{itemData.itemName}";

    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어와의 거리 확인
        CheckPlayerDistance();

        // 상호작용 입력 처리
        if (playerInRange && Input.GetKeyDown(KeyCode.F))
        {
            PickupItem();
        }

        // 시각적 효과 적용
        ApplyVisualEffects();
    }

    private void CheckPlayerDistance()
    {
        GameObject player = PlayerManager.Instance.GetPlayer();

        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance <= interactionRange && !playerInRange)
            {
                // 플레이어가 범위 안에 들어옴
                playerInRange = true;
                ShowInteractionUI();
            }
            else if (distance > interactionRange && playerInRange)
            {
                // 플레이어가 범위 밖으로 나감
                playerInRange = false;
                HideInteractionUI();
            }
        }
    }

    private void ShowInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(true);
        }
    }

    private void HideInteractionUI()
    {
        if (interactionUI != null)
        {
            interactionUI.SetActive(false);
        }
    }

    private void PickupItem()
    {
        if (itemData == null)
        {
            Debug.LogError("아이템 데이터가 없습니다!");
            return;
        }

        // 인벤토리에 아이템 추가 시도
        bool success = InventoryManager.Instance.AddItem(itemId, quantity);

        if (success)
        {
            Debug.Log($"{itemData.itemName} x{quantity} 획득!");

            // 아이템 획득 효과 (선택사항)
            PlayPickupEffect();

            // 아이템 오브젝트 제거
            Destroy(gameObject);
        }
        else
        {
            Debug.LogWarning("인벤토리가 가득 차서 아이템을 획득할 수 없습니다!");
            // 인벤토리 가득참 메시지 표시 (선택사항)
            ShowInventoryFullMessage();
        }
    }

    private void ApplyVisualEffects()
    {
        // 회전 효과
        if (enableRotation)
        {
            transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
        }

        // 위아래 움직임 효과
        if (enableBobbing)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
        }
    }

    private void PlayPickupEffect()
    {
        // 파티클 효과, 사운드 등 추가 가능
        // 예: GetComponent<ParticleSystem>()?.Play();
        // 예: AudioSource.PlayClipAtPoint(pickupSound, transform.position);
    }

    private void ShowInventoryFullMessage()
    {
        // UI 매니저를 통해 "인벤토리가 가득 참" 메시지 표시
        Debug.Log("인벤토리가 가득 참!");
    }

    private void OnDrawGizmosSelected()
    {
        // 상호작용 범위 시각화 (Scene 뷰에서만 보임)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, interactionRange);
    }
}
