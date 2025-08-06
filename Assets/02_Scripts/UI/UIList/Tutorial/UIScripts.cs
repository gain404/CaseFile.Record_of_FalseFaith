using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIScripts : MonoBehaviour
{
    [SerializeField] private GameObject subtitlePanel;
    [SerializeField] private TMP_Text subtitleText;
    [SerializeField] private List<string> promptTextData;
    private Player _player;
    private float _textDelayTime = 0.05f;
    private float _sentenceDelayTime = 0.5f;
    private UIManager _uiManager;

    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
        _uiManager = UIManager.Instance;
    }
    
    private void Start()
    {
        StartCoroutine(StartPrompt());
    }

    private IEnumerator StartPrompt()
    {
        _player.PlayerController.playerActions.Disable();
        subtitlePanel.SetActive(true);
        foreach (var prompt in promptTextData)
        {
            yield return StartCoroutine(PromptText(prompt));
            yield return new WaitForSeconds(_sentenceDelayTime);
        }
        subtitlePanel.SetActive(false);
        _player.PlayerController.playerActions.Enable();
    }

    private IEnumerator PromptText(string text)
    {
        subtitleText.text = "";
        foreach (char c in text)
        {
            subtitleText.text += c;
            yield return new WaitForSeconds(_textDelayTime);
        }
    }
    
}
