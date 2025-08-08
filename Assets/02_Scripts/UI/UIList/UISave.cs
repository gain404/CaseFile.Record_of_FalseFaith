using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISave : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    public GameObject saveUIPanel;
    public Button[] saveSlotButtons = new Button[3];
    public TextMeshProUGUI[] slotInfoTexts = new TextMeshProUGUI[3];


    [Header("설정")]
    public bool pauseGameWhenOpen = true;

    void Start()
    {
        InitializeUI();
        SetupButtonEvents();
    }

    void Update()
    {
        HandleInput();
    }

    private void InitializeUI()
    {
        if (saveUIPanel != null)
        {
            saveUIPanel.SetActive(false);
        }
    }

    private void SetupButtonEvents()
    {
        for (int i = 0; i < saveSlotButtons.Length; i++)
        {
            if (saveSlotButtons[i] != null)
            {
                int slotIndex = i;
                saveSlotButtons[i].onClick.AddListener(() => OnSaveSlotClicked(slotIndex));
            }
        }
    }

    private void HandleInput()
    {
        // ESC키로 UI 닫기
        if (IsUIOpen() && Input.GetKeyDown(KeyCode.Escape))
        {
            CloseSaveUI();
        }
    }

    public void OpenSaveUI()
    {
        if (saveUIPanel != null)
        {
            saveUIPanel.SetActive(true);

            if (pauseGameWhenOpen)
            {
                Time.timeScale = 0f;
            }

            UpdateSlotInfo();
        }
    }

    public void CloseSaveUI()
    {
        if (saveUIPanel != null)
        {
            saveUIPanel.SetActive(false);

            if (pauseGameWhenOpen)
            {
                Time.timeScale = 1f;
            }
        }
    }

    public bool IsUIOpen()
    {
        return saveUIPanel != null && saveUIPanel.activeSelf;
    }

    private void UpdateSlotInfo()
    {
        SaveData[] saveSlots = SaveManager.Instance.GetAllSaveSlots();

        for (int i = 0; i < saveSlots.Length && i < slotInfoTexts.Length; i++)
        {
            if (slotInfoTexts[i] != null)
            {
                if (saveSlots[i] != null)
                {
                    slotInfoTexts[i].text = $"Slot {i + 1}\n" +
                                           $"Player: {saveSlots[i].playerName}\n" +
                                           $"Saved: {saveSlots[i].saveTime:yyyy-MM-dd HH:mm}";
                }
                else
                {
                    slotInfoTexts[i].text = $"Slot {i + 1}\n[Empty]";
                }
            }
        }
    }

    private void OnSaveSlotClicked(int slotIndex)
    {
        // 현재 활성 세이브 포인트에서 데이터 수집
        SavePoint activeSavePoint = GetActiveSavePoint();

        if (activeSavePoint != null)
        {
            SaveData currentData = activeSavePoint.CollectSavePointData();
            SaveManager.Instance.SaveGame(currentData, slotIndex);

            UpdateSlotInfo();
            Debug.Log($"슬롯 {slotIndex + 1}에 저장 완료!");

            CloseSaveUI();
        }
        else
        {
            Debug.LogError("활성 세이브 포인트를 찾을 수 없습니다!");
        }
    }

    private SavePoint GetActiveSavePoint()
    {
        // 현재 플레이어 근처의 세이브 포인트 찾기
        SavePoint[] savePoints = FindObjectsByType<SavePoint>(FindObjectsSortMode.None);//세이브 포인트가 여러개 있으면 정렬 안하고 걍 다 찾는다는 뜻임
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player == null) return null;

        foreach (SavePoint savePoint in savePoints)
        {
            float distance = Vector3.Distance(player.transform.position, savePoint.transform.position);
            if (distance <= 2f) // 적절한 거리 설정
            {
                return savePoint;
            }
        }

        return null;
    }
}
