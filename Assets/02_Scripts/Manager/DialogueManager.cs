using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;
using System;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    // --- UI 요소 ---
    [Header("UI")]
    public GameObject dialoguePanel;
    public GameObject choicePanel;
    public TMP_Text dialogueText;
    public TMP_Text npcNameText;
    public Image playerImage;
    public Image npcImage;
    public GameObject continueArrow;
    public Button[] choiceButtons;

    // --- 카메라 및 효과 ---
    [Header("Cinemachine & Effect")]
    public CinemachineCamera dialogueCamera;
    public CameraSwitcher cameraSwitcher;
    public CanvasGroup dialogueBoxGroup;
    public CanvasGroup playerImageGroup;
    public CanvasGroup npcImageGroup;
    public GameObject backgroundDim;

    // --- 위치 및 애니메이션 설정 ---
    [Header("Settings")]
    public RectTransform dialogueBoxRect;
    public Vector2 playerBoxPos;
    public Vector2 npcBoxPos;
    public float fadeDuration = 0.3f;
    public float moveDuration = 0.4f;
    public float typingSpeed = 0.02f;

    // --- 대화 상태 관리 ---
    public enum DialogueState { Inactive, Transitioning, Typing, WaitingForInput, ShowingChoices, Paused }
    public DialogueState CurrentState = DialogueState.Inactive;

    private DialogueAsset currentDialogue;
    private int currentIndex = 0;
    private Coroutine displayCoroutine;
    private bool isClickLocked = false; // 연속 클릭 방지를 위한 잠금 변수
    private NPCInteraction _currentNpc;
    // 기타 상태 변수
    private string[] currentItemLines;
    private int currentItemIndex = 0;
    private bool isItemDialogue = false;
    public bool IsDialogueFinished { get; private set; }
    private ShopManager _shopManager;
    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }
    private void Start()
    {
        _shopManager = ShopManager.Instance;
    }

    // --- 대화 시작/종료 로직 ---
    public void StartDialogue(DialogueAsset asset, Transform dialogueTarget)
    {
        if (CurrentState != DialogueState.Inactive) return;
        currentDialogue = asset;
        isItemDialogue = false;
        SetCameraTarget(dialogueTarget);
        _currentNpc = dialogueTarget.GetComponent<NPCInteraction>();
        StartDialogueCommon();
    }
    
    public void StartItemDialogue(string[] lines, Transform itemTarget)
    {
        if (CurrentState != DialogueState.Inactive) return;
        currentItemLines = lines;
        isItemDialogue = true;
        
        SetCameraTarget(itemTarget);

        StartDialogueCommon();
    }
    
    private void SetCameraTarget(Transform targetParent)
    {
        if (targetParent != null)
        {
            // 부모 오브젝트(NPC나 아이템) 밑에서 "CinemachineTarget"을 이름으로 찾음
            Transform newTarget = targetParent.Find("CinemachineTarget");
            if (newTarget != null)
            {
                dialogueCamera.Follow = newTarget;
            }
            else
            {
                // CinemachineTarget이 없으면 NPC/아이템 자체를 타겟으로 설정 (예비용)
                dialogueCamera.Follow = targetParent;
                Debug.LogWarning($"'{targetParent.name}' 오브젝트 밑에 'CinemachineTarget'을 찾을 수 없습니다. 부모 오브젝트를 타겟으로 설정합니다.");
            }
        }
    }

    private void StartDialogueCommon()
    {
        currentIndex = 0;
        currentItemIndex = 0;
        IsDialogueFinished = false;

        cameraSwitcher.SwitchToDialogueCamera();
        backgroundDim.SetActive(true);
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        
        if (isItemDialogue)
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
        backgroundDim.SetActive(false);
        cameraSwitcher.SwitchToPlayerCamera();
        dialogueCamera.Follow = null;
        currentDialogue = null;
        currentItemLines = null;
    }

    // --- 핵심 입력 처리 (더욱 단순화) ---
    public void HandleClick()
    {
        // 입력이 잠겨있거나, 입력을 받을 상태가 아니면 무시
        if (isClickLocked || CurrentState != DialogueState.WaitingForInput)
        {
            return;
        }

        // 클릭이 처리되면 즉시 잠그고 다음 대사로 진행
        StartCoroutine(ClickLockout());
        AdvanceDialogue();
    }

    private IEnumerator ClickLockout()
    {
        isClickLocked = true;
        yield return new WaitForSeconds(0.2f); // 0.2초간 추가 클릭 방지
        isClickLocked = false;
    }
    
    // --- 상태 및 UI 관리 ---
    private void SetState(DialogueState newState)
    {
        CurrentState = newState;
        continueArrow.SetActive(newState == DialogueState.WaitingForInput);
    }
    
    private void StopDisplayCoroutine()
    {
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
            displayCoroutine = null;
        }
    }

    // --- 대화 진행 로직 ---
    private void AdvanceDialogue()
    {
        if (isItemDialogue)
        {
            currentItemIndex++;
            if (currentItemIndex >= currentItemLines.Length) { EndDialogue(); return; }
        }
        else
        {
            DialogueLine currentLine = currentDialogue.lines[currentIndex];
            if (currentLine.nextLineIndices != null && currentLine.nextLineIndices.Length > 0)
                currentIndex = currentLine.nextLineIndices[0];
            else
                currentIndex++;
            
            if (currentIndex >= currentDialogue.lines.Length) { EndDialogue(); return; }
        }
        ShowLine();
    }

    private void ShowLine()
    {
        StopDisplayCoroutine();
        displayCoroutine = StartCoroutine(DisplayLineCoroutine());
    }
    
    // --- 코루틴 로직 ---
    private IEnumerator DisplayLineCoroutine()
    {
        SetState(DialogueState.Transitioning);

        string textToDisplay;
        if (isItemDialogue)
        {
            textToDisplay = currentItemLines[currentItemIndex];
            yield return null; 
        }
        else
        {
            DialogueLine line = currentDialogue.lines[currentIndex];
            textToDisplay = line.text;

            // ★★★ 여기가 최종 수정된 부분입니다 ★★★
            // DisplayLineCoroutine 함수 전체를 이 코드로 사용하시면 됩니다.
            if (line.type == DialogueType.OpenStore)
            {
                Debug.Log($"[DialogueManager] OpenStore 라인 진입! NPC: {(_currentNpc != null ? _currentNpc.name : "null")}");
                if (line.shopData != null && _shopManager != null)
                {
                    dialoguePanel.SetActive(false);
                    _shopManager.OpenShop(line.shopData);
                    
                    // '첫 대화 완료'를 알리는 책임은 이제 PlayerInteractState가 가집니다.
                    // 따라서 이 코드는 최종 설계에 따라 삭제됩니다.
                    // if (_currentNpc != null) { _currentNpc.CompleteFirstDialogue(); }
                    
                    PauseDialogue();
                    yield break;
                }
                else
                {
                    Debug.LogError("ShopData 또는 ShopManager가 연결되지 않아 상점을 열 수 없습니다! 다음 대사로 강제 진행합니다.");
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
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = line.choices[i];
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
    
        DialogueLine line = currentDialogue.lines[currentIndex];
        currentIndex = line.nextLineIndices[choiceIndex];

        if (currentIndex >= currentDialogue.lines.Length)
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