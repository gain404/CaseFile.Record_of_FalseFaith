using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    UIInventory, UIDialogue,UIShop,UIHealth,UIStamina,UIPrompt,
    Title
    //ui 붙일까 안붙일까...
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