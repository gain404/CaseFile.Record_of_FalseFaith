using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SavePoint : MonoBehaviour
{
    [Header("세이브 포인트 설정")]
    public GameObject saveUIPanel;
    public Button[] saveSlotButtons = new Button[3];
    public TextMeshProUGUI[] slotInfoTexts = new TextMeshProUGUI[3];
    public TextMeshProUGUI interactionText;

    private bool playerInRange = false;
    //private GameObject player; //한 씬에 여러 세이브 포인트가 있을 경우 오류 발생 플레이어을 직접 찾는 방식으로 변경

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // UI 패널 초기 비활성화
        if (saveUIPanel != null)
        {
            saveUIPanel.SetActive(false);
        }
        // 버튼 이벤트 연결
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            int slotIndex = i;
            saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotClicked(slotIndex));
        }

        if (interactionText != null)
        {
            interactionText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 플레이어가 범위 내에 있고 E키를 눌렀을 때
        if (playerInRange && Input.GetKeyUp(KeyCode.E))
        {
            OpenSaveUI();
        }
        // ESC키로 UI 닫기
        if (saveUIPanel != null && saveUIPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSaveUI();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = true;

            // 상호작용 텍스트 표시
            if (interactionText != null)
            {
                interactionText.gameObject.SetActive(true);
                interactionText.text = "Press E to Save";
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerInRange = false;

            // 상호작용 텍스트 숨기기
            if (interactionText != null)
                interactionText.gameObject.SetActive(false);

            // UI 패널 닫기
            CloseSaveUI();
        }
    }

    private void OpenSaveUI()
    {
        if (saveUIPanel != null)
        {
            saveUIPanel.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지

            // 슬롯 정보 업데이트
            UpdateSlotInfo();
        }
    }

    private void CloseSaveUI()
    {
        if (saveUIPanel != null)
        {
            saveUIPanel.SetActive(false);
            Time.timeScale = 1f; // 게임 재개
        }
    }

    void UpdateSlotInfo()
    {
        SaveData[] saveSlots = SaveManager.Instance.GetAllSaveSlots();

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (slotInfoTexts[i] != null)
            {
                if (saveSlots[i] != null)
                {
                    // 기존 세이브 데이터 정보 표시
                    slotInfoTexts[i].text = $"Slot {i + 1}\n" +
                                           $"Player: {saveSlots[i].playerName}\n" +
                                           $"Saved: {saveSlots[i].saveTime:yyyy-MM-dd HH:mm}";
                }
                else
                {
                    // 빈 슬롯 표시
                    slotInfoTexts[i].text = $"Slot {i + 1}\n[Empty]";
                }
            }
        }
    }

    private void OnSaveSlotClicked(int slotIndex)
    {
        GameObject player = PlayerManager.Instance.GetPlayer();

        if (player == null)
        {
            Debug.LogError("플레이어를 찾을 수 없습니다!");
            return;
        }

        // 현재 플레이어 데이터 수집
        SaveData currentData = CollectPlayerData();

        // 슬롯에 저장
        SaveManager.Instance.SaveGame(currentData, slotIndex);

        // UI 업데이트
        UpdateSlotInfo();

        // 저장 완료 메시지
        Debug.Log($"슬롯 {slotIndex + 1}에 저장 완료!");

        // UI 닫기
        CloseSaveUI();
    }

    SaveData CollectPlayerData()
    {
        SaveData data = new SaveData();

        GameObject player = PlayerManager.Instance.GetPlayer();

        if (player != null)
        {
            // 플레이어 위치 저장
            Vector3 playerPos = player.transform.position;
            data.posX = playerPos.x;
            data.posY = playerPos.y;
            data.posZ = playerPos.z;

            // 현재 씬 이름 저장
            data.sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

            // 추가 플레이어 데이터 수집 (필요에 따라 수정)
            // 예: PlayerStat 컴포넌트에서 체력 등 가져오기

            PlayerStat playerStat = player.GetComponent<PlayerStat>();
            if (playerStat != null)
            {
                data.health = (int) playerStat.CurrentHp;
            }
        }

        return data;
    }
}
