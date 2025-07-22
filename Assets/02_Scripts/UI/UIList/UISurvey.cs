using System;
using UnityEngine;
using UnityEngine.UI;

public class UISurvey : MonoBehaviour
{
    [SerializeField] private Button bookButton;
    [SerializeField] private GameObject uiFile;
    
    private void Start()
    {
        bookButton.onClick.AddListener(OnBook);
        uiFile.SetActive(false);
    }

    public void BookButtonActive()
    {
        bookButton.gameObject.SetActive(true);
    }

    private void OnBook()
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
}
