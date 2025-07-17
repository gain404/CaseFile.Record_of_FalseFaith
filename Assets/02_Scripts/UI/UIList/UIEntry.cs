using UnityEngine;

public enum UIType
{
    Inventory,
    Title
    //ui 붙일까 안붙일까...
}

public enum SceneName
{
    //Scene이름 정확히 작성
}

[System.Serializable]
public class UIEntry
{
    public SceneName sceneName;
    public UIType uiType;
    public GameObject uiPrefab;
}