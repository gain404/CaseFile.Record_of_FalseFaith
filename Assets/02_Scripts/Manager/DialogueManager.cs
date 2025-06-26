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

    private DialogueAsset currentDialogue;
    private int currentIndex = 0;
    private bool isTyping = false;
    private Coroutine typingCoroutine;

    private Dictionary<string, bool> talkedToNPC = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }

        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        continueArrow.SetActive(false);
        backgroundDim.SetActive(false);
    }

    public void StartDialogue(DialogueAsset asset)
    {
        currentDialogue = asset;
        currentIndex = 0;

        if (talkedToNPC.ContainsKey(asset.npcID))
        {
            Debug.Log("이미 대화한 NPC입니다.");
            // 필요한 경우 다른 대사 로직 분기 가능
        }
        else
        {
            talkedToNPC[asset.npcID] = true;
        }
        cameraSwitcher.SwitchToDialogueCamera();
        backgroundDim.SetActive(true);
        
        ShowLine();
    }

    void ShowLine()
    {
        DialogueLine line = currentDialogue.lines[currentIndex];

        SetupDialogueBox(line);

        switch (line.type)
        {
            case DialogueType.NPCLine:
            case DialogueType.PlayerLine:
                dialoguePanel.SetActive(true);
                choicePanel.SetActive(false);
                continueArrow.SetActive(false);
                if (typingCoroutine != null) StopCoroutine(typingCoroutine);
                typingCoroutine = StartCoroutine(TypeText(line.text));
                break;

            case DialogueType.PlayerChoice:
                dialoguePanel.SetActive(false);
                choicePanel.SetActive(true);
                continueArrow.SetActive(false);
                SetChoices(line);
                break;
        }
    }

    IEnumerator TypeText(string text)
    {
        isTyping = true;
        dialogueText.text = "";
        foreach (char c in text)
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.02f); // 타이핑 속도 조절
        }

        isTyping = false;
        continueArrow.SetActive(true);
    }

    void SetChoices(DialogueLine line)
    {
        for (int i = 0; i < choiceButtons.Length; i++)
        {
            if (i < line.choices.Length)
            {
                choiceButtons[i].gameObject.SetActive(true);
                choiceButtons[i].GetComponentInChildren<TMP_Text>().text = line.choices[i];
                int next = line.nextLineIndices[i];
                choiceButtons[i].onClick.RemoveAllListeners();
                choiceButtons[i].onClick.AddListener(() =>
                {
                    currentIndex = next;
                    ShowLine();
                });
            }
            else
            {
                choiceButtons[i].gameObject.SetActive(false);
            }
        }
    }

    void SetupDialogueBox(DialogueLine line)
    {
        npcNameText.text = line.characterName;

        // 위치 이동 연출 등은 별도 Coroutine이나 DOTween으로 처리 가능
        if (line.type == DialogueType.NPCLine)
        {
            // NPC 이미지 fade in, 플레이어 이미지 fade out
        }
        else if (line.type == DialogueType.PlayerLine)
        {
            // 반대 연출
        }

        // 이미지 바꾸기
        npcImage.sprite = line.portrait;
        playerImage.sprite = line.portrait; // 플레이어일 경우에만
    }

    void Update()
    {
        /*if (dialoguePanel.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            OnScreenClick();
        }

        if (Input.GetMouseButtonDown(0))
        {
            OnScreenClick();
        }*/
    }

    void OnScreenClick()
    {
        if (choicePanel.activeSelf) return; // 선택지 중엔 무시

        if (isTyping)
        {
            if (typingCoroutine != null)
            {
                StopCoroutine(typingCoroutine);
                dialogueText.text = currentDialogue.lines[currentIndex].text;
                isTyping = false;
                continueArrow.SetActive(true);
            }
        }
        else if (continueArrow.activeSelf)
        {
            currentIndex++;
            if (currentIndex >= currentDialogue.lines.Length)
            {
                EndDialogue();
            }
            else
            {
                ShowLine();
            }
        }
    }

    void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        choicePanel.SetActive(false);
        backgroundDim.SetActive(false);
        cameraSwitcher.SwitchToPlayerCamera();
    }

    public bool HasTalkedTo(string npcID)
    {
        return talkedToNPC.ContainsKey(npcID);
    }
}
