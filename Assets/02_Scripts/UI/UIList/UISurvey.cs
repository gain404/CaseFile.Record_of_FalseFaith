using System;
using UnityEngine;
using UnityEngine.UI;

public class UISurvey : MonoBehaviour
{
    [SerializeField] private Button bookButton;
    [SerializeField] private GameObject uiBook;

    private void Start()
    {
        bookButton.onClick.AddListener(OnBook);
    }

    private void OnBook()
    {
        uiBook.SetActive(true);
        bookButton.gameObject.SetActive(false);
    }

    private void OffBook()
    {
        uiBook.SetActive(false);
        bookButton.gameObject.SetActive(true);
    }
}
