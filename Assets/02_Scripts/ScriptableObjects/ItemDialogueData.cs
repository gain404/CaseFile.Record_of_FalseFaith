using UnityEngine;

[CreateAssetMenu(fileName = "NewItemDialogue", menuName = "Scriptable Objects/Item Dialogue")]
public class ItemDialogueData : ScriptableObject
{
    [TextArea(2, 5)]
    public string[] dialogueLines; // 여러 줄 지원
}
