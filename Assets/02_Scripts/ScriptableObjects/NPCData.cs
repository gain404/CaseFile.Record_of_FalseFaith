using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "Game/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcID;
    public DialogueAsset dialogueAsset;
}