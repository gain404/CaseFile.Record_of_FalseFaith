using UnityEngine;

[CreateAssetMenu(fileName = "NewNPCData", menuName = "Scriptable Objects/NPC Data")]
public class NPCData : ScriptableObject
{
    public string npcID;
    public DialogueAsset dialogueAsset;
}