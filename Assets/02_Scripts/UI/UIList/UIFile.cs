using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UIFile : MonoBehaviour
{
    public static List<List<bool>> SurveyActive { get; private set; }

    [SerializeField] private GameObject file;
    [SerializeField] private List<Button> chapterButton;
    
    [SerializeField] private List<Button> listButtons;
    //설명 데이터 리스트로 받아오기
    [SerializeField] private List<string> explainNames;
    [SerializeField] private List<string> explains;
    [SerializeField] private TextMeshProUGUI explainNameText;
    [SerializeField] private TextMeshProUGUI explainText;
    [SerializeField] private UISurvey uiSurvey;

    private List<TextMeshProUGUI> _buttonText = new List<TextMeshProUGUI>();
    private void Start()
    {
        for (int i = 0; i < listButtons.Count; i++)
        {
            int capturedIndex = i;
            listButtons[capturedIndex].onClick.AddListener(() => OnExplain(capturedIndex));
            var textMesh = listButtons[capturedIndex].gameObject.GetComponentInChildren<TextMeshProUGUI>();
            _buttonText.Add(textMesh);
            if (SurveyActive[1][capturedIndex])
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
        SurveyActive[chapter][index] = true;
        _buttonText[index].text = explainNames[index];
    }
    
    private void OnExplain(int index)
    {
        explainText.text = explains[index];
        explainNameText.text = explainNames[index];
    }
}
