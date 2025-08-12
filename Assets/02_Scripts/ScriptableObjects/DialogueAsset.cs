using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dialogue/NPC Dialogue")]
public class DialogueAsset : ScriptableObject
{
    public string npcID; 
    public DialogueLine[] lines;

    [System.Serializable]
    public class RandomGroup
    {
        public int baseIndex;
        public int[] indices;
    }

    // 🔹 직렬화용
    public List<RandomGroup> randomGroupList = new List<RandomGroup>();

    // 🔹 런타임용 (직렬화 안 함)
    [System.NonSerialized] 
    public Dictionary<int, int[]> randomGroups;
}