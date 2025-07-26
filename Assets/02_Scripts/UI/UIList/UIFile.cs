using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UIFile : MonoBehaviour
{
    private static List<List<bool>> _surveyActive;

    [SerializeField] private GameObject file;
    [SerializeField] private List<Button> chapterButton;
    [SerializeField] private List<Button> listButtons;
    [SerializeField] private TextMeshProUGUI explainNameText;
    [SerializeField] private TextMeshProUGUI explainText;

    private List<TextMeshProUGUI> _buttonText = new List<TextMeshProUGUI>();
    private void Start()
    {
        if (_surveyActive == null)
        {
            _surveyActive = new List<List<bool>>();
        }
        for (int i = 0; i < listButtons.Count; i++)
        {
            int capturedIndex = i;
            listButtons[capturedIndex].onClick.AddListener(() => OnExplain(capturedIndex));
            var textMesh = listButtons[capturedIndex].gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _buttonText.Add(textMesh);
            if (_surveyActive[1][capturedIndex])
            {
                OnActiveUi(1,capturedIndex);
            }
        }
        for (int j = 0; j < chapterButton.Count; j++)
        {
            int chapter = j;
            chapterButton[j].onClick.AddListener(() => SetButtons(chapter));
        }
    }

    private void SetButtons(int chapter)
    {
        file.transform.SetAsLastSibling();
        listButtons[chapter].gameObject.transform.SetAsLastSibling();
        //데이터 넣기
    }
    
    public void OnActiveUi(int chapter, int index)
    {
        _surveyActive[chapter][index] = true;
    }
    
    private void OnExplain(int index)
    {
        
    }
}
