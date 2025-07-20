using System.Collections.Generic;
using UnityEngine;

public enum UIType
{
    UIInventory, UIDialogue,UIShop,UIHealth,UIStamina,UIPrompt,
    Title
    //ui 붙일까 안붙일까...
}

[System.Serializable]
public class UIEntry
{
    public List<SceneName> sceneName;
    public UIType uiType;
    public GameObject uiPrefab;
}