using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public DialogueType type;

    [TextArea(2, 5)]
    public string text;

    public string characterName;         // NPC 이름 또는 "??"
    public Sprite portrait;            // 각 대사에 맞는 얼굴 스프라이트

    public string[] choices;           // 선택지 텍스트 (선택지일 때만)
    public int[] nextLineIndices;      // 선택지에 따른 다음 인덱스
    
    [Header("Store-Specific")]
    [Tooltip("Type이 OpenStore일 때 연결할 상점 데이터")]
    public ShopData shopData;
}