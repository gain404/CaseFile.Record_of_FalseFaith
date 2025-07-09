using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "ScriptableObjects/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcID;
    
    //기존 dialogueAsset을 아래 두 개로 변경
    [Header("대화 에셋")]
    public DialogueAsset firstDialogueAsset;
    [Tooltip("첫 번째 대화(상점 열기 등)가 끝난 후 보여줄 대화. 없으면 비워두세용.")]
    public DialogueAsset secondDialogueAsset;
}