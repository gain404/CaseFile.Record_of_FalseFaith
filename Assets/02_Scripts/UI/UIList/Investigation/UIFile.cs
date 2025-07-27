using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UIFile : MonoBehaviour
{
    [SerializeField] private List<Button> chapterButton;
    [SerializeField] private List<GameObject> chapterList;
    
    private List<bool> _isChapterOpen = new();

    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            _isChapterOpen.Add(false);
        }
        //밑에는 임시
        ChapterOpen(3);
    }
    
    private void Start()
    {
        for (int i = 0; i < chapterButton.Count; i++)
        {
            int chapter = i;
            chapterButton[i].onClick.AddListener(() => SetChapterList(chapter));
            chapterButton[i].enabled = _isChapterOpen[i];
        }
    }

    private void SetChapterList(int chapter)
    {
        chapterButton[chapter].gameObject.transform.SetAsLastSibling();
        foreach (GameObject list in chapterList)
        {
            list.SetActive(false);
        }
        chapterList[chapter].SetActive(true);
    }

    public void ChapterOpen(int chapter)
    {
        for (int i = 0; i < chapter; i++)
        {
            _isChapterOpen[i] = true;
        }
    }
}
