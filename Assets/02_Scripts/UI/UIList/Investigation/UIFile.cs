using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UIFile : MonoBehaviour
{
    [SerializeField] private GameObject file;
    [SerializeField] private List<Button> chapterButton;
    [SerializeField] private List<Button> listButtons;
    [SerializeField] private TextMeshProUGUI explainNameText;
    [SerializeField] private TextMeshProUGUI explainText;

    private List<TextMeshProUGUI> _buttonText = new List<TextMeshProUGUI>();
    private CsvManager _csvManager;
    private void Start()
    {
        _csvManager = CsvManager.Instance;
        for (int i = 0; i < chapterButton.Count; i++)
        {
            int chapter = i;
            chapterButton[i].onClick.AddListener(() => SetChapterList(chapter));
        }
    }

    private void SetChapterList(int chapter)
    {
        file.transform.SetAsLastSibling();
        listButtons[chapter].gameObject.transform.SetAsLastSibling();
        //데이터 넣기
    }
    
    private void OnExplain(int index)
    {
        
    }
}
