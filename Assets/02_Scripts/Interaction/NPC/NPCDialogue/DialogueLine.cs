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
    public ShopData shopData;

    public int baseIndex;                  // 그룹 기준 인덱스
    public int[] randomGroupIndices;       // 같은 baseIndex 후보들
}