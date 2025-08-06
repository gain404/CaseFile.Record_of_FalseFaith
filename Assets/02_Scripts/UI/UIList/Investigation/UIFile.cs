using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UIFile : MonoBehaviour
{
    [SerializeField] private GameObject file;
    [SerializeField] private List<Button> chapterButton;
    [SerializeField] private List<GameObject> chapterList;
    [SerializeField] private Button testButton;
    
    private List<bool> _isChapterOpen = new();
    private CsvManager _csvManager;
    private List<ChapterInvestigationList> _chapterInvestigationList = new();

    private void Awake()
    {
        _csvManager = CsvManager.Instance;
        foreach (var chapter in chapterList)
        {
            _chapterInvestigationList.Add(chapter.GetComponent<ChapterInvestigationList>());
        }
        
        for (int i = 0; i < 5; i++)
        {
            _isChapterOpen.Add(false);
        }
        testButton.onClick.AddListener(Test);
        //밑에는 임시
        SetChapterList(0);
        ChapterOpen(3);
    }
    
    public void FileInit()
    {
        foreach (var list in _chapterInvestigationList)
        {
            list.Init();
        }
        
        for (int i = 0; i < chapterButton.Count; i++)
        {
            int chapter = i;
            chapterButton[i].onClick.AddListener(() => SetChapterList(chapter));
            chapterButton[i].enabled = _isChapterOpen[i];
        }
    }

    private void SetChapterList(int chapter)
    {
        file.transform.SetAsLastSibling();
        chapterButton[chapter].gameObject.transform.SetAsLastSibling();
        foreach (GameObject list in chapterList)
        {
            list.SetActive(false);
        }
        chapterList[chapter].SetActive(true);
    }

    //chapter버튼 열기
    public void ChapterOpen(int chapter)
    {
        for (int i = 0; i < chapter; i++)
        {
            _isChapterOpen[i] = true;
        }
    }

    private void Test()
    {
        OpenInvestigationList(101);
        OpenInvestigationList(102);
        OpenInvestigationList(103);
        OpenInvestigationList(104);
        OpenInvestigationList(201);
        OpenInvestigationList(301);
    }
    
    public void OpenInvestigationList(int index)
    {
        if (_csvManager.InvestigationData.TryGetValue(index, out InvestigationData data))
        {
            data.isOpen = true;
            _chapterInvestigationList[data.chapter - 1].ShowButtonText(index);
        }
    } 
}
