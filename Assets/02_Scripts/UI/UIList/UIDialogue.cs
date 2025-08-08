using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using System;
using System.Linq;

public enum DialogueState { Inactive, Transitioning, Typing, WaitingForInput, ShowingChoices, Paused }

public class UIDialogue : MonoBehaviour
{
    public DialogueState CurrentState { get; private set; }
    public bool IsDialogueFinished { get; private set; }
    public bool autoAdvanced;
    
    [Header("UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private GameObject choicePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text npcNameText;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image npcImage;
    [SerializeField] private GameObject continueArrow;
    [SerializeField] private Button[] choiceButtons;
    
    [Header("Effect")]
    [SerializeField] private CanvasGroup playerImageGroup;
    [SerializeField] private CanvasGroup npcImageGroup;
    
    [Header("Settings")]
    [SerializeField] private RectTransform dialogueBoxRect;
    [SerializeField] private Vector2 defaultBoxPos;
    [SerializeField] private Vector2 playerBoxPos;
    [SerializeField] private Vector2 npcBoxPos;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float typingSpeed;
    
    private DialogueAsset _currentDialogue;
    private int _currentIndex;
    private Coroutine _displayCoroutine;
    private bool _isClickLocked;

    private CinemachineCamera _dialogueCamera;
    private string[] _currentItemLines;
    private int _currentItemIndex;
    private bool _isItemDialogue;
    private UIShop _uiShop;
    private TMP_Text[] _buttonTexts;
    private FadeManager _fadeManager;

    // 🔹 조사 후 세컨드 대화를 위해 NPC 정보 저장
    private NPCData _currentNpcData;
    private Transform _currentNpcTransform;

    private void Start()
    {
        _uiShop = UIManager.Instance.UIShop;
        CurrentState = DialogueState.Inactive;
        _buttonTexts = new TMP_Text[choiceButtons.Length];
        for (int i = 0; i < choiceButtons.Length; i++)
            _buttonTexts[i] = choiceButtons[i].GetComponentInChildren<TMP_Text>();

        GameObject dialogueCamera = GameObject.FindGameObjectWithTag("DialogueCamera");
        _dialogueCamera = dialogueCamera.GetComponent<CinemachineCamera>();
        _fadeManager = FadeManager.Instance;
        EndDialogue();
    }

    // --- 대화 시작/종료 ---
    public void StartDialogue(DialogueAsset asset, Transform dialogueTarget, NPCData npcData = null)
    {
        if (CurrentState != DialogueState.Inactive) return;
        _currentDialogue = asset;
        _isItemDialogue = false;
        _currentNpcData = npcData;
        _currentNpcTransform = dialogueTarget;

        SetCameraTarget(dialogueTarget);
        StartDialogueCommon();
    }
    
    public void StartItemDialogue(string[] lines, Transform itemTarget)
    {
        if (CurrentState != DialogueState.Inactive) return;
        _currentItemLines = lines;
        _isItemDialogue = true;
        SetCameraTarget(itemTarget);
        StartDialogueCommon();
    }
    
    private void SetCameraTarget(Transform targetParent)
    {
        if (targetParent != null)
        {
            Transform newTarget = null;
            foreach (Transform child in targetParent)
            {
                if (child.name == "CinemachineTarget")
                {
                    newTarget = child;
                    break;
                }
            }
            _dialogueCamera.Follow = newTarget != null ? newTarget : targetParent;
        }
    }

    private void StartDialogueCommon()
    {
        _currentIndex = 0;
        _currentItemIndex = 0;
        IsDialogueFinished = false;

        _dialogueCamera.Priority = 30;
        _fadeManager.OrderChange(0);
        _fadeManager.Fade(0.5f,0.1f);
        DialogueLine firstLine = _currentDialogue.lines[0];
        if (firstLine.type == DialogueType.OpenStore || firstLine.type == DialogueType.StartInvestigation)
        {
            
            dialoguePanel.SetActive(false);
            PauseDialogue();

            if (firstLine.type == DialogueType.OpenStore && _uiShop != null)
            {
                _uiShop.OpenShop(firstLine.shopData);
            }
            else if (firstLine.type == DialogueType.StartInvestigation)
            {
                UIInventory inventory = FindObjectOfType<UIInventory>();
                if (inventory != null)
                {
                    inventory.EnterInvestigationMode();
                }
            }
        }
        // ✨ 일반 대화일 경우에만 대화창을 활성화
        else
        {
            dialoguePanel.SetActive(true);
            dialogueBoxRect.anchoredPosition = defaultBoxPos;

            if (_isItemDialogue)
            {
                npcNameText.text = "";
                playerImage.gameObject.SetActive(false);
                npcImage.gameObject.SetActive(false);
            }
            ShowLine(); // 일반 대화 흐름 시작
        }
        choicePanel.SetActive(false);
        dialogueBoxRect.anchoredPosition = defaultBoxPos;

        if (_isItemDialogue)
        {
            npcNameText.text = "";
            playerImage.gameObject.SetActive(false);
            npcImage.gameObject.SetActive(false);
        }
        ShowLine();
    }
    
    private void EndDialogue()
    {
        SetState(DialogueState.Inactive);
        IsDialogueFinished = true; 
        dialogueText.text = "";
        npcNameText.text = "";
        continueArrow.SetActive(false);
        choicePanel.SetActive(false);

        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);

        dialoguePanel.SetActive(false);
        _fadeManager.Fade(0,0.1f);

        _dialogueCamera.Priority = 0;
        _dialogueCamera.Follow = null;

        _currentDialogue = null;
        _currentItemLines = null;
    }

    // --- 조사 전용: 대화 강제 종료 후 세컨드 대화 시작 ---
    public void ForceEndAndStartSecondDialogue()
    {
        EndDialogue();

        if (_currentNpcData != null && _currentNpcData.secondDialogueAsset != null)
        {
            Debug.Log("[Dialogue] 세컨드 대화 시작");
            StartDialogue(_currentNpcData.secondDialogueAsset, _currentNpcTransform, _currentNpcData);
        }
    }

    // --- 입력 처리 ---
    public void HandleClick()
    {
        if (_isClickLocked || CurrentState != DialogueState.WaitingForInput)
            return;

        StartCoroutine(ClickLockout());
        AdvanceDialogue();
    }

    private IEnumerator ClickLockout()
    {
        _isClickLocked = true;
        yield return new WaitForSeconds(0.2f);
        _isClickLocked = false;
    }
    
    private void SetState(DialogueState newState)
    {
        CurrentState = newState;
        continueArrow.SetActive(newState == DialogueState.WaitingForInput);
    }
    
    private void StopDisplayCoroutine()
    {
        if (_displayCoroutine != null)
        {
            StopCoroutine(_displayCoroutine);
            _displayCoroutine = null;
        }
    }

    // --- 모든 대사 이동을 중앙에서 처리 ---
    private void GoToLine(int targetIndex)
    {
        if (targetIndex >= _currentDialogue.lines.Length)
        {
            EndDialogue();
            return;
        }

        if (_currentDialogue.randomGroups == null &&
            _currentDialogue.randomGroupList != null &&
            _currentDialogue.randomGroupList.Count > 0)
        {
            _currentDialogue.randomGroups = _currentDialogue.randomGroupList
                .ToDictionary(g => g.baseIndex, g => g.indices);
        }

        int baseIdx = _currentDialogue.lines[targetIndex].baseIndex;
        int finalIndex = targetIndex;

        if (_currentDialogue.randomGroups != null &&
            _currentDialogue.randomGroups.TryGetValue(baseIdx, out var group))
        {
            int randomPick = UnityEngine.Random.Range(0, group.Length);
            finalIndex = group[randomPick];
            Debug.Log($"[Dialogue] 랜덤 그룹 발견! BaseIndex={baseIdx}, 후보: [{string.Join(",", group)}] → 선택: {finalIndex}");
        }

        _currentIndex = finalIndex;
        ShowLine();
    }

    private void AdvanceDialogue()
    {
        DialogueLine currentLine = _currentDialogue.lines[_currentIndex];
        int nextIndex = (currentLine.nextLineIndices?.Length > 0)
            ? currentLine.nextLineIndices[0]
            : _currentIndex + 1;

        Debug.Log($"[Dialogue] AdvanceDialogue 호출됨 → 다음 목표 {nextIndex}");
        GoToLine(nextIndex);
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        choicePanel.SetActive(false);
        SetState(DialogueState.Transitioning);

        DialogueLine line = _currentDialogue.lines[_currentIndex];
        int targetIndex = line.nextLineIndices[choiceIndex];
        Debug.Log($"[Dialogue] 선택지 {choiceIndex} 선택 → 목표 {targetIndex}");

        GoToLine(targetIndex);
    }

    private void ShowLine()
    {
        StopDisplayCoroutine();
        _displayCoroutine = StartCoroutine(DisplayLineCoroutine());
    }

    private IEnumerator DisplayLineCoroutine()
    {
        SetState(DialogueState.Transitioning);

        string textToDisplay;
        if (_isItemDialogue)
        {
            textToDisplay = _currentItemLines[_currentItemIndex];
            yield return null;
        }
        else
        {
            DialogueLine line = _currentDialogue.lines[_currentIndex];
            textToDisplay = line.text;

            if (line.type == DialogueType.OpenStore)
            {
                if (line.shopData != null && _uiShop != null)
                {
                    // ✨ [수정] UI를 열기 전에, 기존 초상화를 먼저 정리합니다.
                    yield return StartCoroutine(FadeOutImages());
                    npcNameText.text = ""; // 이름표도 초기화
                
                    dialoguePanel.SetActive(false);
                    _uiShop.OpenShop(line.shopData);
                    PauseDialogue();
                    yield break;
                }
                else
                {
                    AdvanceDialogue();
                    yield break;
                }
            }

            // StartInvestigation 처리
            if (line.type == DialogueType.StartInvestigation)
            {
                Debug.Log("[Dialogue] StartInvestigation 호출됨 → 인벤토리 조사 모드 진입");
        
                // ✨ [수정] 인벤토리를 열기 전에, 기존 초상화를 먼저 정리합니다.
                yield return StartCoroutine(FadeOutImages());
                npcNameText.text = ""; // 이름표도 초기화

                // 1) 대화 패널 닫기 (이것만으로는 부족)
                dialoguePanel.SetActive(false);

                // 2) 인벤토리 조사 모드 열기
                UIInventory inventory = FindObjectOfType<UIInventory>();
                if (inventory != null)
                {
                    inventory.EnterInvestigationMode();
                }

                // 3) 대화 일시정지
                PauseDialogue();
                yield break;
            }

            yield return StartCoroutine(TransitionSpeaker(line));

            if (line.type == DialogueType.PlayerChoice)
            {
                dialogueText.text = textToDisplay;
                SetupChoices(line);
                SetState(DialogueState.ShowingChoices);
                yield break;
            }
        }

        yield return StartCoroutine(TypeTextCoroutine(textToDisplay));
        if (autoAdvanced) //자동 진행 모드일 경우 (컷씬)
        {
            yield return new WaitForSeconds(1.5f);
            AdvanceDialogue();
        }
        else
        {
            SetState(DialogueState.WaitingForInput);
        }
    }


    private IEnumerator TypeTextCoroutine(string text)
    {
        SetState(DialogueState.Typing);
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    public void PauseDialogue() => SetState(DialogueState.Paused);

    public void ResetDialogueState()
    {
        SetState(DialogueState.Inactive);
        IsDialogueFinished = true; 
        continueArrow.SetActive(false);
        choicePanel.SetActive(false);
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        dialogueText.text = "";
        npcNameText.text = "";
        _dialogueCamera.Priority = 0;
        _dialogueCamera.Follow = null;
    
        _currentDialogue = null;
        _currentItemLines = null;
    }

    #region Other Methods
    private IEnumerator TransitionSpeaker(DialogueLine line)
    {
        bool isPlayer = (line.type == DialogueType.PlayerLine || line.type == DialogueType.PlayerChoice);
        Vector2 targetPos = isPlayer ? playerBoxPos : npcBoxPos;
        if (dialogueBoxRect.anchoredPosition != targetPos)
        {
            yield return StartCoroutine(FadeOutImages());
            yield return StartCoroutine(MoveDialogueBox(targetPos));
        }
        SetupPortraitsAndName(line, isPlayer);
        if ((isPlayer && playerImageGroup.alpha == 0) || (!isPlayer && npcImageGroup.alpha == 0))
        {
            yield return StartCoroutine(FadeInImages(line.type));
        }
    }
    private void SetupPortraitsAndName(DialogueLine line, bool isPlayer)
    {
        npcNameText.text = line.characterName;
        playerImage.gameObject.SetActive(isPlayer);
        npcImage.gameObject.SetActive(!isPlayer);
        if (isPlayer) playerImage.sprite = line.portrait;
        else npcImage.sprite = line.portrait;
    }
    private void SetupChoices(DialogueLine line)
    {
        choicePanel.SetActive(true);
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            bool isActive = i < line.choices.Length;
            choiceButtons[i].gameObject.SetActive(isActive);
            if (isActive)
            {
                _buttonTexts[i].text = line.choices[i];
                int choiceIndex = i;
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
        }
    }
    IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha)
    {
        float startAlpha = group.alpha;
        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }
        group.alpha = targetAlpha;
    }
    IEnumerator FadeOutImages()
    {
        yield return StartCoroutine(FadeCanvasGroup(playerImageGroup, 0f));
        yield return StartCoroutine(FadeCanvasGroup(npcImageGroup, 0f));
    }
    IEnumerator FadeInImages(DialogueType type)
    {
        if (type == DialogueType.PlayerLine || type == DialogueType.PlayerChoice)
            yield return StartCoroutine(FadeCanvasGroup(playerImageGroup, 1f));
        else if (type == DialogueType.NPCLine)
            yield return StartCoroutine(FadeCanvasGroup(npcImageGroup, 1f));
    }
    IEnumerator MoveDialogueBox(Vector2 targetPos)
    {
        Vector2 startPos = dialogueBoxRect.anchoredPosition;
        for (float t = 0; t < moveDuration; t += Time.deltaTime)
        {
            dialogueBoxRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / moveDuration);
            yield return null;
        }
        dialogueBoxRect.anchoredPosition = targetPos;
    }
    #endregion
}
