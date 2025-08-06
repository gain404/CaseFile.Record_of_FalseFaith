using System;
using System.Collections.Generic;
using DG.Tweening;
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
    [SerializeField] private Button testButton;

    private Player _player;
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
        testButton.onClick.AddListener(() => OnTutorialPanel(TutorialType.Fight));
        mainPanel.SetActive(false);
    }

    public void OnTutorialPanel(TutorialType tutorialType)
    {
        _player.PlayerController.playerActions.Disable();
        mainPanel.SetActive(true);

        foreach (TutorialList tutorial in tutorialList)
        {
            if (tutorial.tutorialPanel != null)
            {
                tutorial.tutorialPanel.SetActive(tutorial.tutorialType == tutorialType);
            }
        }

        DOVirtual.DelayedCall(2.0f, OffTutorialPanel);

    }

    private void OffTutorialPanel()
    {
        _player.PlayerController.playerActions.Enable();
        mainPanel.SetActive(false);
    }
}
