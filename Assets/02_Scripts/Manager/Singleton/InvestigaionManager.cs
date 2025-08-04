using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class InvestigationManager : Singleton<InvestigationManager>
{
    [Header("Timer UI")]
    [SerializeField] private GameObject timerUI;
    [SerializeField] private Image timerFill;
    [SerializeField] private float investigationDuration = 30f;

    private bool _isInvestigating;
    private int _currentItemIndex;
    private float _timeRemaining;

    private UIInvestigation _uiInvestigation;  // 조사 파일 UI
    private UIInventory _inventory;           // 실제 인벤토리 UI
    private UIDialogue _uiDialogue;           // 대화 UI

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
        if (_isInvestigating) return; // 중복 방지

        _isInvestigating = true;
        _currentItemIndex = itemIndex;

        // 타이머 시작
        timerUI.SetActive(true);
        _timeRemaining = investigationDuration;
        timerFill.fillAmount = 1f;
        StartCoroutine(TimerRoutine());

        Debug.Log($"[Investigation] 아이템 {itemIndex} 조사 시작. {investigationDuration}초 후 완료.");

        // Second Dialogue로 전환
        if (_uiDialogue != null)
        {
            // UIDialogue 내부에 구현 필요
            _uiDialogue.ForceEndAndStartSecondDialogue();
        }

        // 조사 모드 종료 → 인벤토리 닫기
        if (_inventory != null)
        {
            _inventory.Toggle(); // 인벤토리 닫기
        }
    }
    //조사 타이머
    private IEnumerator TimerRoutine()
    {
        while (_timeRemaining > 0f)
        {
            _timeRemaining -= Time.deltaTime;
            timerFill.fillAmount = _timeRemaining / investigationDuration;
            yield return null;
        }

        EndInvestigation();
    }

    /// <summary>
    /// 조사 완료 처리
    /// </summary>
    private void EndInvestigation()
    {
        timerUI.SetActive(false);
        _isInvestigating = false;

        // 조사 파일에 정보 추가
        if (_uiInvestigation != null)
        {
            _uiInvestigation.GetInvestigation(_currentItemIndex);
        }

        Debug.Log($"[Investigation] 아이템 {_currentItemIndex} 조사 완료!");
    }
}
