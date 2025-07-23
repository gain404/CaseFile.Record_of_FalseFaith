using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;

public class UIFile : MonoBehaviour
{
    public static List<List<bool>> SurveyActive { get; private set; }

    [SerializeField] private GameObject file;
    [SerializeField] private List<Button> chapterButton;
    
    [SerializeField] private List<Button> buttons;
    //설명 데이터 리스트로 받아오기
    [SerializeField] private List<string> explainNames;
    [SerializeField] private List<string> explains;
    [SerializeField] private List<TextMeshProUGUI> buttonText;
    [SerializeField] private TextMeshProUGUI explainNameText;
    [SerializeField] private TextMeshProUGUI explainText;
    [SerializeField] private UISurvey uiSurvey;
    
    private void Start()
    {
        for (int i = 0; i < buttons.Count; i++)
        {
            int capturedIndex = i;
            buttons[i].onClick.AddListener(() => OnExplain(capturedIndex));
            buttonText[i] = buttons[i].GetComponent<TextMeshProUGUI>();
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
        buttons[chapter].gameObject.transform.SetAsLastSibling();
        //데이터 넣기
    }
    
    public void OnActiveUi(int chapter, int index)
    {
        SurveyActive[chapter][index] = true;
        buttonText[index].text = explainNames[index];
    }
    
    private void OnExplain(int index)
    {
        explainText.text = explains[index];
        explainNameText.text = explainNames[index];
    }
}
