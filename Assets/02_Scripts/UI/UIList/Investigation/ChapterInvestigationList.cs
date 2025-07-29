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
    private readonly List<Button> _investigationButton = new();
    private readonly List<TextMeshProUGUI> _buttonText = new();
    private readonly List<int> _buttonIndex = new();
    public void Init()
    {
        _csvManager = CsvManager.Instance;
        CreateButtons();
        for (int i = 0;i<_investigationButton.Count;i++)
        {
            int buttonIndex = i;
            _investigationButton[buttonIndex].onClick.AddListener(() => SetExplainPanel(buttonIndex));
            Debug.Log(buttonIndex + "버튼 시스너 성공");
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
                    _buttonText[i].text = "? ? ? ? ? ? ?";
                    _investigationButton[i].enabled = false;
                }
            }
        }
    }

    private void CreateButtons()
    {
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
        int index = _buttonIndex[buttonIndex];
        if (_csvManager.InvestigationData.TryGetValue(index,out InvestigationData data))
        {
            investigationObjectName.text = data.name;
            investigationObjectExplain.text = data.content;
        }
    }

    public void ShowButtonText(int index)
    {
        int listNum = _buttonIndex.IndexOf(index);
        if (_csvManager.InvestigationData.TryGetValue(index, out InvestigationData data))
        {
            _buttonText[listNum].text = data.name;
            _investigationButton[listNum].enabled = true;
        }
    }
}
