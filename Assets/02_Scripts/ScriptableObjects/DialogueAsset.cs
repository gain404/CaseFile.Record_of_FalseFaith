using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Dialogue")]
public class DialogueAsset : ScriptableObject
{
    public string npcID;
    public DialogueLine[] lines;

    // baseIndex별 랜덤 후보 맵
    public Dictionary<int, int[]> randomGroups;
}
