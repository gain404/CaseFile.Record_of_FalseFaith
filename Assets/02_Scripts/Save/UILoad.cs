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

    private void Start()
    {
        if (loadUIPanel != null) loadUIPanel.SetActive(false);

        for (int i = 0; i < loadSlotButtons.Length; i++)
        {
            int slot = i;
            loadSlotButtons[i].onClick.AddListener(() => OnLoadSlotClicked(slot));
        }

        if (backButton != null)
            backButton.onClick.AddListener(CloseLoadUI);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
            OpenLoadUI();
    }

    public void OpenLoadUI()
    {
        if (loadUIPanel == null) return;
        loadUIPanel.SetActive(true);
        Time.timeScale = 0f;
        UpdateLoadSlotInfo();
    }

    public void CloseLoadUI()
    {
        if (loadUIPanel == null) return;
        loadUIPanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void UpdateLoadSlotInfo()
    {
        var saveSlots = SaveManager.Instance.GetAllSaveSlots();

        for (int i = 0; i < saveSlots.Length && i < loadSlotInfoTexts.Length; i++)
        {
            if (loadSlotInfoTexts[i] == null) continue;

            if (saveSlots[i] != null)
            {
                loadSlotInfoTexts[i].text =
                    $"Slot {i + 1}\n" +
                    $"Player: {saveSlots[i].playerName}\n" +
                    $"Scene: {saveSlots[i].sceneName}\n" +
                    $"Saved: {saveSlots[i].saveTime:yyyy-MM-dd HH:mm:ss}";
                loadSlotButtons[i].interactable = true;
            }
            else
            {
                loadSlotInfoTexts[i].text = $"Slot {i + 1}\n[Empty]";
                loadSlotButtons[i].interactable = false;
            }
        }
    }

    private void OnLoadSlotClicked(int slotIndex)
    {
        if (!SaveManager.Instance.HasSaveFile(slotIndex))
        {
            Debug.LogWarning($"슬롯 {slotIndex + 1} 데이터 없음");
            return;
        }

        // UI 닫고 시간 원복
        Time.timeScale = 1f;
        CloseLoadUI();

        // ▶ SaveManager가 내부에서 LoadingBar.LoadScene 호출 + 자동 복원
        SaveManager.Instance.LoadGameAndScene(slotIndex);
    }
}
