using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "ScriptableObjects/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcID;
    public DialogueAsset dialogueAsset;
}