using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISurvey : MonoBehaviour
{
    [SerializeField] private Button bookButton;
    [SerializeField] private GameObject uiFile;

    private UIFile _uiFile;
    
    private void Start()
    {
        bookButton.onClick.AddListener(ActiveBook);
        uiFile.SetActive(false);
        _uiFile = uiFile.GetComponent<UIFile>();
    }

    //조사 시트 버튼을 active시켜주는 메써드
    public void BookButtonActive()
    {
        bookButton.gameObject.SetActive(true);
    }

    private void ActiveBook()
    {
        if (!uiFile.activeSelf)
        {
            uiFile.SetActive(true);
        }
        else
        {
            uiFile.SetActive(false);
        }
    }
    

    //이부에서 정보를 수집했을 때 호출되는 메써드
    public void GetSurvey(int chapter, int index)
    {
        _uiFile.OnActiveUi(chapter, index);
    }
}
