using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

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
    // 여러 bool 대신 하나의 enum으로 상태를 명확하게 관리
    private enum DialogueState { Inactive, Transitioning, Typing, WaitingForInput, ShowingChoices }
    private DialogueState currentState = DialogueState.Inactive;

    private DialogueAsset currentDialogue;
    private int currentIndex = 0;
    
    // 코루틴 참조를 저장하여 더 안전하게 제어
    private Coroutine displayCoroutine;

    // 기타 상태 변수
    private string[] currentItemLines;
    private int currentItemIndex = 0;
    private bool isItemDialogue = false;
    public bool IsDialogueFinished { get; private set; }
    private Dictionary<string, bool> talkedToNPC = new();


    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;
    }

    public void StartDialogue(DialogueAsset asset)
    {
        if (currentState != DialogueState.Inactive) return;

        currentDialogue = asset;
        currentIndex = 0;
        isItemDialogue = false;
        IsDialogueFinished = false;

        if (!talkedToNPC.ContainsKey(asset.npcID))
        {
            talkedToNPC[asset.npcID] = true;
        }

        cameraSwitcher.SwitchToDialogueCamera();
        backgroundDim.SetActive(true);

        ShowLine();
    }
    
    public void StartItemDialogue(string[] lines)
    {
        // 이미 다른 대화가 진행 중이면 실행하지 않음
        if (currentState != DialogueState.Inactive) return;

        // 아이템 대화 모드로 전환
        isItemDialogue = true;
        currentItemLines = lines;
        currentItemIndex = 0;
        IsDialogueFinished = false;

        // UI 초기화 (아이템 대화는 화자 정보가 없음)
        backgroundDim.SetActive(true);
        cameraSwitcher.SwitchToDialogueCamera();
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        npcNameText.text = "";
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);

        // 메인 흐름인 ShowLine()을 호출하여 대사 표시 시작
        ShowLine();
    }

    public void HandleClick()
    {
        // switch 문을 사용하여 현재 상태에 따른 동작을 명확하게 처리
        switch (currentState)
        {
            case DialogueState.Typing:
                // 타이핑 중일 때 클릭하면 전체 텍스트 표시
                FinishTyping();
                break;

            case DialogueState.WaitingForInput:
                // 입력 대기 중일 때 클릭하면 다음 줄로 진행
                AdvanceDialogue();
                break;
        }
    }

    private void AdvanceDialogue()
    {
        // 아이템 대화 진행 로직
        if (isItemDialogue)
        {
            currentItemIndex++;
            if (currentItemIndex >= currentItemLines.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowLine();
            }
            return;
        }
        
        DialogueLine currentLine = currentDialogue.lines[currentIndex];
    
        if (currentLine.nextLineIndices != null && currentLine.nextLineIndices.Length > 0)
        {
            currentIndex = currentLine.nextLineIndices[0];
        }
        else
        {
            currentIndex++;
        }

        if (currentIndex >= currentDialogue.lines.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowLine();
        }
    }

    private void ShowLine()
    {
        // 이전에 실행 중이던 대사 표시 코루틴을 안전하게 중지
        if (displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        }
        displayCoroutine = StartCoroutine(DisplayLineCoroutine());
    }
    
    private IEnumerator DisplayLineCoroutine()
    {
        currentState = DialogueState.Transitioning;

        // UI 초기화
        continueArrow.SetActive(false);
        dialoguePanel.SetActive(true);

        // 아이템 대화일 경우, 텍스트만 가져오고 복잡한 전환은 생략
        if (isItemDialogue)
        {
            choicePanel.SetActive(false);
            string itemText = currentItemLines[currentItemIndex];
            yield return StartCoroutine(TypeTextCoroutine(itemText));
        }
        // 일반 대화일 경우, 기존 로직 수행
        else
        {
            choicePanel.SetActive(false);
            DialogueLine line = currentDialogue.lines[currentIndex];

            yield return StartCoroutine(TransitionSpeaker(line));

            if (line.type == DialogueType.PlayerChoice)
            {
                dialogueText.text = line.text;
                SetupChoices(line);
                currentState = DialogueState.ShowingChoices;
            }
            else
            {
                yield return StartCoroutine(TypeTextCoroutine(line.text));
            }
        }
    }
    
    // 화자 전환 시 애니메이션 처리 로직을 별도 함수로 분리
    private IEnumerator TransitionSpeaker(DialogueLine line)
    {
        // 화자 변경 감지 (이전 로직은 복잡하여 단순화)
        bool isPlayer = (line.type == DialogueType.PlayerLine || line.type == DialogueType.PlayerChoice);
        Vector2 targetPos = isPlayer ? playerBoxPos : npcBoxPos;

        // 대화창 위치가 다를 때만 이동
        if (dialogueBoxRect.anchoredPosition != targetPos)
        {
            yield return StartCoroutine(FadeOutImages());
            yield return StartCoroutine(MoveDialogueBox(targetPos));
        }

        SetupPortraitsAndName(line, isPlayer);

        // 이미지가 비활성화 되어있다면 Fade In
        if ((isPlayer && playerImageGroup.alpha == 0) || (!isPlayer && npcImageGroup.alpha == 0))
        {
            yield return StartCoroutine(FadeInImages(line.type));
        }
    }

    private void SetupPortraitsAndName(DialogueLine line, bool isPlayer)
    {
        npcNameText.text = line.characterName;
        
        // 비활성화된 이미지 먼저 끄고 필요한 이미지만 설정
        playerImage.gameObject.SetActive(isPlayer);
        npcImage.gameObject.SetActive(!isPlayer);

        if (isPlayer) playerImage.sprite = line.portrait;
        else npcImage.sprite = line.portrait;
    }

    private IEnumerator TypeTextCoroutine(string text)
    {
        currentState = DialogueState.Typing;
        dialogueText.text = "";

        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(typingSpeed);
        }
        
        FinishTyping();
    }
    
    private void FinishTyping()
    {
        if (currentState == DialogueState.Typing && displayCoroutine != null)
        {
            StopCoroutine(displayCoroutine);
        
            // 아이템/일반 대화에 맞는 전체 텍스트를 즉시 표시
            if (isItemDialogue)
            {
                dialogueText.text = currentItemLines[currentItemIndex];
            }
            else
            {
                dialogueText.text = currentDialogue.lines[currentIndex].text;
            }
        }
    
        currentState = DialogueState.WaitingForInput;
        continueArrow.SetActive(true);
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
                
                int choiceIndex = i; // 클로저 문제 방지
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() => OnChoiceSelected(choiceIndex));
            }
        }
    }

    private void OnChoiceSelected(int choiceIndex)
    {
        // 선택 후에는 UI 상태를 비활성으로 변경
        currentState = DialogueState.Inactive;
        
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
    
    private void EndDialogue()
    {
        currentState = DialogueState.Inactive;
        IsDialogueFinished = true; 
        
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        backgroundDim.SetActive(false);
        cameraSwitcher.SwitchToPlayerCamera();

        // 리소스 정리
        currentDialogue = null;
        isItemDialogue = false;
        currentItemLines = null;
    }

    #region Coroutines for Animation (기존과 거의 동일)
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
        {
            yield return StartCoroutine(FadeCanvasGroup(playerImageGroup, 1f));
        }
        else if (type == DialogueType.NPCLine)
        {
            yield return StartCoroutine(FadeCanvasGroup(npcImageGroup, 1f));
        }
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

    public bool HasTalkedTo(string npcID) => talkedToNPC.ContainsKey(npcID);
}