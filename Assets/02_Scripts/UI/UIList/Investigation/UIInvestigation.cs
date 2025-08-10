using UnityEngine;
using UnityEngine.UI;

public class UIInvestigation : MonoBehaviour
{
    [SerializeField] private Button bookButton;
    [SerializeField] private GameObject uiFile;
    
    private UIFile _uiFile;
    
    private void Start()
    {
        bookButton.onClick.AddListener(ActiveBook);
        _uiFile = uiFile.GetComponent<UIFile>();
        uiFile.SetActive(false);
        _uiFile.FileInit();
    }

    //조사 시트 버튼을 active시켜주는 메써드
    public void OnBookButton()
    {
        bookButton.gameObject.SetActive(!bookButton.gameObject.activeSelf);
    }

    private void ActiveBook()
    {
        uiFile.SetActive(!uiFile.activeSelf);
    }
    

    //이부에서 정보를 수집했을 때 호출되는 메써드
    public void GetInvestigation(int index)
    {
        _uiFile.OpenInvestigationList(index);
    }

    public void StartChapter(int chapter)
    {
        _uiFile.ChapterOpen(chapter);
    }
}
