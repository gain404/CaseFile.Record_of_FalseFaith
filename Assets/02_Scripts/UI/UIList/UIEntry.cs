using UnityEngine;

public enum UIType
{
    UIInventory, UIDialogue,UIShop,
    Title
    //ui 붙일까 안붙일까...
}

[System.Serializable]
public class UIEntry
{
    public string sceneName;
    public UIType uiType;
    public GameObject uiPrefab;
}