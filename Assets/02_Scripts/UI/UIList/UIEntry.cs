using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    UIInventory, UIDialogue,UIShop,UIHealth,UIStamina,UIPrompt,UIInvestigation, UISave, UILoad, UIMap, UIGuideIcon,
    Title
}

public enum SceneNaem
{
    Chapter1,Chapter2,Chapter3
}

[System.Serializable]
public class UIEntry
{
    public List<SceneNaem> sceneName;
    public UIType uiType;
    public GameObject uiPrefab;
}