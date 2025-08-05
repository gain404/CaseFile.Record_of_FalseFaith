using System.Collections;
using UnityEngine;
using TMPro;

public class InvestigationManager : Singleton<InvestigationManager>
{
    [Header("Timer UI")]
    [SerializeField] private GameObject timerUI;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private float investigationDuration = 30f;

    private bool _isInvestigating;
    private int _currentItemIndex;
    private float _timeRemaining;

    private UIInvestigation _uiInvestigation;
    private UIInventory _inventory;
    private UIDialogue _uiDialogue;

    protected override void Awake()
    {
        base.Awake();
        timerUI.SetActive(false);
    }

    private void Start()
    {
        _uiInvestigation = FindObjectOfType<UIInvestigation>();
        _inventory = FindObjectOfType<UIInventory>();
        _uiDialogue = FindObjectOfType<UIDialogue>();
    }

    public void StartInvestigation(int itemIndex)
    {
        if (_isInvestigating) return;

        _isInvestigating = true;
        _currentItemIndex = itemIndex;

        // 타이머 시작
        timerUI.SetActive(true);
        _timeRemaining = investigationDuration;
        UpdateTimerText();
        StartCoroutine(TimerRoutine());

        Debug.Log($"[Investigation] 아이템 {itemIndex} 조사 시작. {investigationDuration}초 후 완료.");

        // 세컨드 대사 전환 후 대화 종료
        if (_uiDialogue != null)
        {
            _uiDialogue.ForceEndAndStartSecondDialogue();
        }

        // 조사 모드 종료 → 인벤토리 닫기
        if (_inventory != null)
        {
            _inventory.ExitInvestigationMode();
        }
    }

    private IEnumerator TimerRoutine()
    {
        while (_timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;
            UpdateTimerText();
            yield return null;
        }

        EndInvestigation();
    }

    private void UpdateTimerText()
    {
        int seconds = Mathf.CeilToInt(_timeRemaining);
        timerText.text = seconds.ToString();
    }

    private void EndInvestigation()
    {
        timerUI.SetActive(false);
        _isInvestigating = false;

        if (_uiInvestigation != null)
        {
            _uiInvestigation.GetInvestigation(_currentItemIndex);
        }

        Debug.Log($"[Investigation] 아이템 {_currentItemIndex} 조사 완료!");
    }
}
