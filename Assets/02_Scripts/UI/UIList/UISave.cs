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
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("[UISave] Player 태그 오브젝트를 찾을 수 없습니다.");
            return null;
        }

        // 비활성 포함해서 모두 수집 (Unity 6)
        SavePoint[] savePoints = FindObjectsByType<SavePoint>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        Debug.Log($"[UISave] 발견한 SavePoint 개수: {savePoints.Length} / PlayerPos:{player.transform.position}");

        SavePoint nearest = null;
        float best = float.MaxValue;

        foreach (var sp in savePoints)
        {
            float d = Vector3.Distance(player.transform.position, sp.transform.position);
            Debug.Log($"[UISave] {sp.name} @ {sp.transform.position}, dist:{d}, activeSelf:{sp.gameObject.activeSelf}");
            if (d < best)
            {
                best = d;
                nearest = sp;
            }
        }

        if (nearest != null && best <= 5f) // 임계값 잠시 키워서 확인
        {
            Debug.Log($"[UISave] 선택된 세이브 포인트: {nearest.name}, dist:{best}");
            return nearest;
        }

        Debug.LogWarning("[UISave] 가까운 세이브 포인트가 범위 내에 없습니다.");
        return null;
    }
}
