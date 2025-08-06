using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public DialogueType type;

    [TextArea(2, 5)]
    public string text;

    public string characterName;         
    public Sprite portrait;            

    public string[] choices;           
    public int[] nextLineIndices;      
    
    [Header("Store-Specific")]
    [Tooltip("Type이 OpenStore일 때 연결할 상점 데이터")]
    public ShopData shopData;

    //  랜덤 그룹용 baseIndex
    public int baseIndex;
}