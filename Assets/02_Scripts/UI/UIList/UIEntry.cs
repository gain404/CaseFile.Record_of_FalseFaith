using UnityEngine;

public enum UIType
{
    Inventory,
    Title
}

[System.Serializable]
public class UIEntry
{
    public UIType uiType;
    public GameObject uiPrefab;
}