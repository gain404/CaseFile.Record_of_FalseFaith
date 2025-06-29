using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Dialogue")]
public class DialogueAsset : ScriptableObject
{
    public string npcID; // NPC 고유 ID 추가
    public DialogueLine[] lines;
}