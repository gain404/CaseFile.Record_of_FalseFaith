using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Cinemachine;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance { get; private set; }

    [Header("UI")]
    public GameObject dialoguePanel;
    public GameObject choicePanel;
    public TMP_Text dialogueText;
    public TMP_Text npcNameText;
    public Image playerImage;
    public Image npcImage;
    public GameObject continueArrow;
    public Button[] choiceButtons;

    [Header("Cinemachine")]
    public CinemachineCamera dialogueCamera;
    public Camera mainCamera;
    public CameraSwitcher cameraSwitcher;

    [Header("Effect")]
    public CanvasGroup dialogueBoxGroup;
    public CanvasGroup playerImageGroup;
    public CanvasGroup npcImageGroup;
    public GameObject backgroundDim;

    [Header("Dialogue Box Positions")]
    public RectTransform dialogueBoxRect;
    public Vector2 playerBoxPos;
    public Vector2 npcBoxPos;

    [Header("Fade & Move Settings")]
    public float fadeDuration = 0.3f;
    public float moveDuration = 0.4f;

    private DialogueAsset currentDialogue;
    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private string[] currentItemLines;
    private int currentItemIndex = 0;
    private bool isItemDialogue = false;
    public bool IsDialogueFinished { get; private set; }

    private Dictionary<string, bool> talkedToNPC = new();
    private DialogueType? lastDialogueType = null; // 직전 대사의 화자 타입


    void Awake()
    {
        if (Instance != null && Instance != this) Destroy(this);
        else Instance = this;

        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        continueArrow.SetActive(false);
        backgroundDim.SetActive(false);
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);
    }

    public void StartDialogue(DialogueAsset asset)
    {
        currentDialogue = asset;
        currentIndex = 0;
        isItemDialogue = false;
        IsDialogueFinished = false;

        if (!talkedToNPC.ContainsKey(asset.npcID))
            talkedToNPC[asset.npcID] = true;

        cameraSwitcher.SwitchToDialogueCamera();
        backgroundDim.SetActive(true);
        ShowLine();
    }

    public void StartItemDialogue(string[] lines)
    {
        isItemDialogue = true;
        currentItemLines = lines;
        currentItemIndex = 0;
        IsDialogueFinished = false;
        backgroundDim.SetActive(true);
        cameraSwitcher.SwitchToDialogueCamera();
        dialoguePanel.SetActive(true);
        choicePanel.SetActive(false);
        npcNameText.text = "";
        npcImage.sprite = null;
        playerImage.sprite = null;

        if (typingCoroutine != null) StopCoroutine(typingCoroutine);
        typingCoroutine = StartCoroutine(TypeText(lines[0]));
    }

    private void ShowLine()
    {
        if (isItemDialogue) return;

        DialogueLine line = currentDialogue.lines[currentIndex];
        StopAllCoroutines();
        StartCoroutine(TransitionDialogueUI(line));
    }

    private IEnumerator TransitionDialogueUI(DialogueLine line)
    {
        bool isSameSpeaker = lastDialogueType.HasValue && lastDialogueType.Value == line.type;
        lastDialogueType = line.type;

        if (!isSameSpeaker)
        {
            yield return StartCoroutine(FadeOutImages());

            Vector2 targetPos = (line.type == DialogueType.PlayerLine || line.type == DialogueType.PlayerChoice)
                ? playerBoxPos : npcBoxPos;
            yield return StartCoroutine(MoveDialogueBox(targetPos));

            SetupPortrait(line);
            npcNameText.text = line.characterName;

            yield return StartCoroutine(FadeInImages(line.type));
        }
        else
        {
            // 같은 화자의 경우 포트레잇만 교체
            SetupPortrait(line);
            npcNameText.text = line.characterName;
        }

        if (line.type == DialogueType.PlayerChoice)
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(true);
            continueArrow.SetActive(false);
            SetChoices(line);
        }
        else
        {
            dialoguePanel.SetActive(true);
            choicePanel.SetActive(false);
            continueArrow.SetActive(false);

            if (typingCoroutine != null) StopCoroutine(typingCoroutine);
            typingCoroutine = StartCoroutine(TypeText(line.text));
        }
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;
        continueArrow.SetActive(true);
    }

    private void SetChoices(DialogueLine line)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < line.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = line.choices[i];

                int nextIndex = line.nextLineIndices[i];
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    currentIndex = nextIndex;
                    ShowLine();
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    public void HandleClick()
    {
        if (choicePanel.activeSelf) 
            return;

        if (isTyping)
        {
            StopCoroutine(typingCoroutine);
            dialogueText.text = isItemDialogue
                ? currentItemLines[currentItemIndex]
                : currentDialogue.lines[currentIndex].text;
            isTyping = false;
            continueArrow.SetActive(true);
            return;
        }

        if (!continueArrow.activeSelf) return;

        if (isItemDialogue)
        {
            currentItemIndex++;
            if (currentItemIndex >= currentItemLines.Length)
                EndDialogue();
            else
            {
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeText(currentItemLines[currentItemIndex]));
            }
        }
        else
        {
            DialogueLine line = currentDialogue.lines[currentIndex];
            currentIndex = (line.nextLineIndices != null && line.nextLineIndices.Length > 0)
                ? line.nextLineIndices[0]
                : currentIndex + 1;

            if (currentIndex >= currentDialogue.lines.Length)
                EndDialogue();
            else
                ShowLine();
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        backgroundDim.SetActive(false);
        cameraSwitcher.SwitchToPlayerCamera();

        isItemDialogue = false;
        currentItemLines = null;
        lastDialogueType = null; // 다음 대화에서 처음부터 처리되도록 초기화
        IsDialogueFinished = true; 
        npcImageGroup.alpha = 0f;
        playerImageGroup.alpha = 0f;
        npcImage.gameObject.SetActive(false);
        playerImage.gameObject.SetActive(false);
        npcNameText.text = "";
    }


    IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha)
    {
        float startAlpha = group.alpha;
        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t / fadeDuration);
            yield return null;
        }
        group.alpha = targetAlpha;
    }

    IEnumerator FadeOutImages()
    {
        yield return StartCoroutine(FadeCanvasGroup(playerImageGroup, 0f));
        yield return StartCoroutine(FadeCanvasGroup(npcImageGroup, 0f));
        playerImage.gameObject.SetActive(false);
        npcImage.gameObject.SetActive(false);
    }

    IEnumerator FadeInImages(DialogueType type)
    {
        if (type == DialogueType.PlayerLine || type == DialogueType.PlayerChoice)
        {
            playerImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(playerImageGroup, 1f));
        }
        else if (type == DialogueType.NPCLine)
        {
            npcImage.gameObject.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(npcImageGroup, 1f));
        }
    }

    IEnumerator MoveDialogueBox(Vector2 targetPos)
    {
        Vector2 startPos = dialogueBoxRect.anchoredPosition;
        float t = 0f;
        while (t < moveDuration)
        {
            t += Time.deltaTime;
            dialogueBoxRect.anchoredPosition = Vector2.Lerp(startPos, targetPos, t / moveDuration);
            yield return null;
        }
        dialogueBoxRect.anchoredPosition = targetPos;
    }

    void SetupPortrait(DialogueLine line)
    {
        if (line.type == DialogueType.PlayerLine || line.type == DialogueType.PlayerChoice)
            playerImage.sprite = line.portrait;
        else if (line.type == DialogueType.NPCLine)
            npcImage.sprite = line.portrait;
    }

    public bool HasTalkedTo(string npcID) => talkedToNPC.ContainsKey(npcID);
}
