using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    UIInventory, UIDialogue,UIShop,UIHealth,UIStamina,UIPrompt,
    UIInvestigation, UISave, UILoad, UIMap, UIGuideIcon,UITutorial, UIScript, UIObjective, UIObjectiveCompleteNotifier, UIGameOverEffect,
    Title, UIInvestigationTimer, UIFadePanel, UIEndingPanel
}

public enum SceneName
{
    Title,Tutorial,Chapter1,Chapter2,Chapter3
}

[System.Serializable]
public class UIEntry
{
    public List<SceneName> sceneName;
    public UIType uiType;
    public GameObject uiPrefab;
}