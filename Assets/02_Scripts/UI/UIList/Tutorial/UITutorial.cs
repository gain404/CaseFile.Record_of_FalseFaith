using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum TutorialType
{
    Move, Fight, Investigation, Interaction
}

[System.Serializable]
public class TutorialList
{
    public TutorialType tutorialType;
    public GameObject tutorialPanel;
}

public class UITutorial : MonoBehaviour
{
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private List<TutorialList> tutorialList;
    [SerializeField] private Button xButton;
    [SerializeField] private Button testButton;

    private void Start()
    {
        xButton.onClick.AddListener(OffTutorialPanel);
        testButton.onClick.AddListener(() => OnTutorialPanel(TutorialType.Fight));
        mainPanel.SetActive(false);
    }

    public void OnTutorialPanel(TutorialType tutorialType)
    {
        mainPanel.SetActive(true);
        foreach (TutorialList tutorial in tutorialList)
        {
            if (tutorial.tutorialPanel != null)
            {
                tutorial.tutorialPanel.SetActive(tutorial.tutorialType == tutorialType);
            }
        }
        Time.timeScale = 0;
    }

    private void OffTutorialPanel()
    {
        mainPanel.SetActive(false);
        Time.timeScale = 1;
    }
}
