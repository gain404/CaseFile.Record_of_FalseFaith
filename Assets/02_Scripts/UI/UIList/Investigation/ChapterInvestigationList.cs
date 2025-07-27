using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChapterInvestigationList : MonoBehaviour
{
    [SerializeField] private int chapterNum;
    [SerializeField] private GameObject content;
    [SerializeField] private TextMeshProUGUI investigationObjectName;
    [SerializeField] private TextMeshProUGUI investigationObjectExplain;
    [SerializeField] private GameObject buttonPrefab;

    private CsvManager _csvManager;
    private List<Button> _investigationButton;
    private List<TextMeshProUGUI> _buttonText;
    private List<int> _buttonIndex;
    private void Start()
    {
        _csvManager = CsvManager.Instance;
        InitUI();
    }

    private void InitUI()
    {
        CreateButtons();
        for (int i = 0;i<_investigationButton.Count;i++)
        {
            int buttonIndex = i;
            _investigationButton[buttonIndex].onClick.AddListener(() => SetExplainPanel(buttonIndex));
        }
        SetButton();
        investigationObjectName.text = " ";
        investigationObjectExplain.text = " ";
    }

    private void SetButton()
    {
        for (int i = 0; i < _buttonIndex.Count; i++)
        {
            if (_csvManager.InvestigationData.TryGetValue(_buttonIndex[i], out InvestigationData data))
            {
                if (data.isOpen)
                {
                    _buttonText[i].text = data.name;
                    _investigationButton[i].enabled = true;
                }
                else
                {
                    _buttonText[i].text = "? ? ? ? ?";
                    _investigationButton[i].enabled = false;
                }
            }
        }
    }

    private void CreateButtons()
    {
        _investigationButton.Clear();
        foreach (Transform child in content.transform)
        {
            Destroy(child.gameObject);
        }
        foreach (var data in _csvManager.InvestigationData)
        {
            if (data.Value.chapter == chapterNum)
            {
                _buttonIndex.Add(data.Key);
            }
        }
        
        for (int i = 0; i < _buttonIndex.Count; i++)
        {
            GameObject newButtonObj = Instantiate(buttonPrefab, content.transform);
            Button button = newButtonObj.GetComponent<Button>();
            if (button != null)
            {
                _investigationButton.Add(button);
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                _buttonText.Add(buttonText);
            }
        }

        if (_investigationButton.Count != _buttonIndex.Count)
        {
            Debug.Log("누락된 파일이 있습니다");
        }
    }
    
    private void SetExplainPanel(int buttonIndex)
    {
        if (_csvManager.InvestigationData.TryGetValue(buttonIndex,out InvestigationData data))
        {
            investigationObjectName.text = data.name;
            investigationObjectExplain.text = data.content;
        }
    }
}
