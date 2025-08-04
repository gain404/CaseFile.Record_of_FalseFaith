using System.Collections.Generic;
using UnityEngine;

public class TriggerObjectiveDataLoader : MonoBehaviour
{
    [SerializeField] private TextAsset triggerGoalCSV;

    // itemId -> List of triggered questIds
    private Dictionary<string, List<int>> triggerMap = new Dictionary<string, List<int>>();

    public void LoadTriggerData()
    {
        if (triggerGoalCSV == null) return;

        string[] lines = triggerGoalCSV.text.Split('\n');
        for (int i = 1; i < lines.Length; i++)
        {
            string[] values = lines[i].Trim().Split(',');
            if (values.Length < 2) continue;

            string itemId = values[0];
            int questId = int.Parse(values[1]);

            if (!triggerMap.ContainsKey(itemId))
                triggerMap[itemId] = new List<int>();

            triggerMap[itemId].Add(questId);
        }
    }

    public List<int> GetTriggeredQuests(string itemId)
    {
        if (triggerMap.TryGetValue(itemId, out var list))
            return list;
        return new List<int>();
    }
}
