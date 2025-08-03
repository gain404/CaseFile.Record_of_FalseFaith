using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using System;
public enum DialogueState { Inactive, Transitioning, Typing, WaitingForInput, ShowingChoices, Paused }
public class UIDialogue : MonoBehaviour
{
    public DialogueState CurrentState { get; private set; }
    public bool IsDialogueFinished { get; private set; }
    
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
    [SerializeField] private Vector2 playerBoxPos;
    [SerializeField] private Vector2 npcBoxPos;
    [SerializeField] private float fadeDuration;
    [SerializeField] private float moveDuration;
    [SerializeField] private float typingSpeed;
    
    private DialogueAsset _currentDialogue;
    private int _currentIndex;
    private Coroutine _displayCoroutine;
    private bool _isClickLocked; // 연속 클릭 방지를 위한 잠금 변수
    // 기타 상태 변수
    private CinemachineCamera _dialogueCamera;
    private string[] _currentItemLines;
    private int _currentItemIndex;
    private bool _isItemDialogue;
    private UIShop _uiShop;
    private TMP_Text[] _buttonTexts;
    private FadeManager _fadeManager;
    private void Start()
    {
        _uiShop = UIManager.Instance.UIShop;
        CurrentState = DialogueState.Inactive;
        _buttonTexts = new TMP_Text[choiceButtons.Length];
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            _buttonTexts[i] = choiceButtons[i].GetComponentInChildren<TMP_Text>();
        }
        GameObject dialogueCamera = GameObject.FindGameObjectWithTag("DialogueCamera");
        _dialogueCamera = dialogueCamera.GetComponent<CinemachineCamera>();
        _fadeManager = FadeManager.Instance;
        EndDialogue();
    }

    // --- 대화 시작/종료 로직 ---
    public void StartDialogue(DialogueAsset asset, Transform dialogueTarget)
    {
        if (CurrentState != DialogueState.Inactive) return;
        _currentDialogue = asset;
        _isItemDialogue = false;
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
            // 부모 오브젝트(NPC나 아이템) 밑에서 "CinemachineTarget"을 이름으로 찾음
            Transform newTarget = null;
            foreach (Transform child in targetParent)
            {
                if (child.name == "CinemachineTarget")
                {
                    newTarget = child;
                    break;
                }
            }
            if (newTarget != null)
            {
                _dialogueCamera.Follow = newTarget;
            }
            else
            {
                // CinemachineTarget이 없으면 NPC/아이템 자체를 타겟으로 설정 (예비용)
                _dialogueCamera.Follow = targetParent;
            }
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
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        
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
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        _fadeManager.Fade(0,0.1f);
        _dialogueCamera.Priority = 0;
        _dialogueCamera.Follow = null;
        _currentDialogue = null;
        _currentItemLines = null;
    }

    // --- 핵심 입력 처리 (더욱 단순화) ---
    public void HandleClick()
    {
        // 입력이 잠겨있거나, 입력을 받을 상태가 아니면 무시
        if (_isClickLocked || CurrentState != DialogueState.WaitingForInput)
        {
            return;
        }

        // 클릭이 처리되면 즉시 잠그고 다음 대사로 진행
        StartCoroutine(ClickLockout());
        AdvanceDialogue();
    }

    private IEnumerator ClickLockout()
    {
        _isClickLocked = true;
        yield return new WaitForSeconds(0.2f); // 0.2초간 추가 클릭 방지
        _isClickLocked = false;
    }
    
    // --- 상태 및 UI 관리 ---
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

    // --- 대화 진행 로직 ---
    private void AdvanceDialogue()
    {
        Debug.Log($"[Dialogue] AdvanceDialogue 호출됨, currentIndex={_currentIndex}");

        DialogueLine currentLine = _currentDialogue.lines[_currentIndex];
        int nextIndex = (currentLine.nextLineIndices?.Length > 0)
            ? currentLine.nextLineIndices[0]
            : _currentIndex + 1;

        Debug.Log($"[Dialogue] nextIndex 초기값={nextIndex}");

        if (nextIndex < _currentDialogue.lines.Length)
        {
            DialogueLine nextLine = _currentDialogue.lines[nextIndex];

            if (nextLine.randomGroupIndices != null && nextLine.randomGroupIndices.Length > 1)
            {
                int randomPick = UnityEngine.Random.Range(0, nextLine.randomGroupIndices.Length);
                nextIndex = nextLine.randomGroupIndices[randomPick];

                Debug.Log($"[Dialogue] baseIndex={nextLine.baseIndex}, 후보={string.Join(",", nextLine.randomGroupIndices)} → 선택={nextIndex}");
            }
        }

        _currentIndex = nextIndex;

        if (_currentIndex >= _currentDialogue.lines.Length)
        {
            Debug.Log("[Dialogue] 대화 종료");
            EndDialogue();
            return;
        }

        ShowLine();
    }





    private void ShowLine()
    {
        StopDisplayCoroutine();
        _displayCoroutine = StartCoroutine(DisplayLineCoroutine());
    }
    
    // --- 코루틴 로직 ---
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

            // ★★★ 여기가 최종 수정된 부분입니다 ★★★
            // DisplayLineCoroutine 함수 전체를 이 코드로 사용하시면 됩니다.
            if (line.type == DialogueType.OpenStore)
            {
                if (line.shopData != null && _uiShop != null)
                {
                    dialoguePanel.SetActive(false);
                    _uiShop.OpenShop(line.shopData);
                    
                    // '첫 대화 완료'를 알리는 책임은 이제 PlayerInteractState가 가집니다.
                    // 따라서 이 코드는 최종 설계에 따라 삭제됩니다.
                    // if (_currentNpc != null) { _currentNpc.CompleteFirstDialogue(); }
                    
                    PauseDialogue();
                    yield break;
                }
                else
                {
                    AdvanceDialogue();
                    yield break;
                }
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
        SetState(DialogueState.WaitingForInput);
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
    public void PauseDialogue()
    {
        SetState(DialogueState.Paused);
    }

    public void ResetDialogueState()
    {
        StopAllCoroutines(); // 혹시 모를 코루틴 정지
        SetState(DialogueState.Inactive);
        IsDialogueFinished = true; // 대화가 끝났다고 확실히 명시
        dialoguePanel.SetActive(false);
    }
    
    #region Other Methods (화자/선택지/애니메이션 - 수정 없음)
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
    private void OnChoiceSelected(int choiceIndex)
    {
        choicePanel.SetActive(false);

        SetState(DialogueState.Inactive);
    
        DialogueLine line = _currentDialogue.lines[_currentIndex];
        _currentIndex = line.nextLineIndices[choiceIndex];

        if (_currentIndex >= _currentDialogue.lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
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