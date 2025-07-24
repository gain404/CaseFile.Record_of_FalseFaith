using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILoad : MonoBehaviour
{

    [Header("로드 UI 설정")]
    public GameObject loadUIPanel;
    public Button[] loadSlotButtons = new Button[3];
    public TextMeshProUGUI[] loadSlotInfoTexts = new TextMeshProUGUI[3];
    public Button backButton;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        // UI 패널 초기 비활성화
        if (loadUIPanel != null)
            loadUIPanel.SetActive(false);

        // 로드 버튼 이벤트 연결
        for (int i = 0; i < loadSlotButtons.Length; i++)
        {
            int slotIndex = i;
            loadSlotButtons[i].onClick.AddListener(() => OnLoadSlotClicked(slotIndex));
        }

        // 뒤로가기 버튼 이벤트 연결
        if (backButton != null)
            backButton.onClick.AddListener(CloseLoadUI);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            UILoad loadSystem = FindAnyObjectByType<UILoad>();
            loadSystem.OpenLoadUI();
        }
    }


    public void OpenLoadUI()
    {
        if (loadUIPanel != null)
        {
            loadUIPanel.SetActive(true);
            Time.timeScale = 0f; // 게임 일시정지

            // 슬롯 정보 업데이트
            UpdateLoadSlotInfo();
        }
    }

    public void CloseLoadUI()
    {
        if (loadUIPanel != null)
        {
            loadUIPanel.SetActive(false);
            Time.timeScale = 1f; // 시간 다시 흐르게
        }
    }

    private void UpdateLoadSlotInfo()
    {
        SaveData[] saveSlots = SaveManager.Instance.GetAllSaveSlots();

        for (int i = 0; i < saveSlots.Length; i++)
        {
            if (loadSlotInfoTexts[i] != null)
            {
                if (saveSlots[i] != null)
                {
                    // 기존 세이브 데이터 정보 표시
                    loadSlotInfoTexts[i].text = $"Slot {i + 1}\n" +
                                               $"Player: {saveSlots[i].playerName}\n" +
                                               $"Scene: {saveSlots[i].sceneName}\n" +
                                               $"Saved: {saveSlots[i].saveTime:yyyy-MM-dd HH:mm:ss}";

                    // 버튼 활성화
                    loadSlotButtons[i].interactable = true;
                }
                else
                {
                    // 빈 슬롯 표시
                    loadSlotInfoTexts[i].text = $"Slot {i + 1}\n[Empty]";

                    // 버튼 비활성화
                    loadSlotButtons[i].interactable = false;
                }
            }
        }
    }

    private void OnLoadSlotClicked(int slotIndex)
    {
        if (SaveManager.Instance.HasSaveFile(slotIndex))
        {
            Debug.Log($"슬롯 {slotIndex + 1}에서 로드 중...");

            // 게임 시간 복원
            Time.timeScale = 1f;

            // 게임 로드 및 씬 이동
            SaveManager.Instance.LoadGameAndScene(slotIndex);
        }
        else
        {
            Debug.LogWarning($"슬롯 {slotIndex + 1}에 저장된 데이터가 없습니다!");
        }
    }
}
