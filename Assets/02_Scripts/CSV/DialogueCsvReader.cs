// DialogueCsvReader.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DialogueCsvReader : MonoBehaviour
{
    [SerializeField] private string dialogueCsvFileName = "DialogueData";

    private void Awake()
    {
        if (FindObjectsOfType<DialogueCsvReader>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        InitializeDialogues();
    }

    private void InitializeDialogues()
    {
        var npcAssets = Resources.LoadAll<NPCData>("");
        var npcMap = npcAssets.ToDictionary(n => n.npcID);
        var grouped = ParseCsvAndGroupById();

        foreach (var kvp in grouped)
        {
            string dialogueID = kvp.Key;
            // ★ 수정된 부분 1: 튜플 구조가 바뀌었으므로 .line을 선택하여 lineList를 생성합니다.
            var lineList = kvp.Value.OrderBy(x => x.lineIndex).Select(x => x.line).ToArray();

            int underscoreIndex = dialogueID.LastIndexOf('_');
            if (underscoreIndex < 0)
            {
                Debug.LogError($"DialogueID '{dialogueID}' 는 '_First' 또는 '_Second' 접미사가 필요합니다.");
                continue;
            }

            string npcID = dialogueID.Substring(0, underscoreIndex);
            string suffix = dialogueID.Substring(underscoreIndex + 1);

            string dialogueAssetPath = $"GeneratedDialogues/{npcID}_{suffix}";
            var dialogueAsset = Resources.Load<DialogueAsset>(dialogueAssetPath);

#if UNITY_EDITOR
            if (dialogueAsset == null)
            {
                dialogueAsset = ScriptableObject.CreateInstance<DialogueAsset>();
                dialogueAsset.npcID = npcID;
                dialogueAsset.lines = lineList;
                SaveAssetToFile(dialogueAsset, npcID, suffix);
            }
            else
            {
                dialogueAsset.npcID = npcID;
                dialogueAsset.lines = lineList;
                EditorUtility.SetDirty(dialogueAsset);
            }
#endif

            // ★ 수정된 부분 2: 랜덤 그룹 생성 로직을 훨씬 정교하게 변경합니다.
            var randomGroups = kvp.Value
                // 1. 먼저, rawIndex에 '_'가 포함된 라인만 필터링하여 랜덤 그룹 후보로 한정합니다.
                .Where(tuple => tuple.rawIndex.Contains("_")) 
                // 2. 그 후보들 내에서 baseIndex를 기준으로 그룹화합니다.
                .GroupBy(tuple => tuple.line.baseIndex)
                // 3. 그룹 내 멤버가 2개 이상인 경우에만 최종 랜덤 그룹으로 인정합니다.
                .Where(g => g.Count() > 1)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(tuple => tuple.lineIndex).ToArray()
                );
            
            // 직렬화용 리스트 생성
            dialogueAsset.randomGroupList = randomGroups
                .Select(g => new DialogueAsset.RandomGroup { baseIndex = g.Key, indices = g.Value })
                .ToList();

            // 런타임 딕셔너리 세팅
            dialogueAsset.randomGroups = randomGroups;

            if (npcMap.TryGetValue(npcID, out var npcData))
            {
                var loadedAsset = Resources.Load<DialogueAsset>(dialogueAssetPath);
                if (suffix.Equals("First", StringComparison.OrdinalIgnoreCase))
                    npcData.firstDialogueAsset = loadedAsset;
                else if (suffix.Equals("Second", StringComparison.OrdinalIgnoreCase))
                    npcData.secondDialogueAsset = loadedAsset;

#if UNITY_EDITOR
                EditorUtility.SetDirty(npcData);
#endif
            }
            else
            {
                Debug.LogWarning($"NPCData for ID '{npcID}' not found.");
            }
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }
    

private Dictionary<string, List<(int lineIndex, DialogueLine line, string rawIndex)>> ParseCsvAndGroupById()
{
    var result = new Dictionary<string, List<(int, DialogueLine, string)>>();
    var csv = Resources.Load<TextAsset>(dialogueCsvFileName);
    if (csv == null)
    {
        Debug.LogError("CSV 파일을 찾을 수 없습니다.");
        return result;
    }

    // ★ 수정: autoIndex를 관리하는 Dictionary 추가
    var autoIndexCounters = new Dictionary<string, int>();

    using (StringReader reader = new StringReader(csv.text))
    {
        string line;
        bool isFirst = true;
        // int autoIndex = 0; // <- 이 줄을 삭제합니다.

        while ((line = reader.ReadLine()) != null)
        {
            if (isFirst) { isFirst = false; continue; }
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] parts = SplitCsvLine(line);
            if (parts.Length < 5) continue;

            string dialogueID = parts[0].Trim();

            // ★ 수정: DialogueID별로 autoIndex를 관리합니다.
            if (!autoIndexCounters.ContainsKey(dialogueID))
            {
                autoIndexCounters[dialogueID] = 0;
            }
            int autoIndex = autoIndexCounters[dialogueID];


            string rawIndex = parts[1].Trim();

            // ★ 수정: baseIndex의 기본값을 autoIndex로 설정하는 부분을
            // rawIndex가 비어있을 때만으로 한정하여 더 명확하게 만듭니다.
            int baseIndex;
            if (string.IsNullOrEmpty(rawIndex))
            {
                baseIndex = autoIndex;
            }
            else
            {
                string[] split = rawIndex.Split('_');
                int.TryParse(split[0], out baseIndex);
            }
            
            // (이하 DialogueLine 생성 부분은 기존과 동일)
            Enum.TryParse(parts[2], out DialogueType type);
            string charName = parts[3];
            string text = parts[4];

            var dialogueLine = new DialogueLine
            {
                type = type,
                characterName = charName,
                text = text,
                baseIndex = baseIndex
            };

            if (parts.Length > 5 && !string.IsNullOrWhiteSpace(parts[5]))
                dialogueLine.portrait = Resources.Load<Sprite>(parts[5].Trim());

            if (parts.Length > 6 && !string.IsNullOrWhiteSpace(parts[6]))
                dialogueLine.choices = parts[6].Split('|').Select(s => s.Trim()).ToArray();

            if (parts.Length > 7 && !string.IsNullOrWhiteSpace(parts[7]))
                dialogueLine.nextLineIndices = parts[7].Split('|')
                    .Select(s => int.TryParse(s.Trim(), out var i) ? i : -1)
                    .Where(i => i >= 0).ToArray();

            if (parts.Length > 8 && !string.IsNullOrWhiteSpace(parts[8]))
                dialogueLine.shopData = Resources.Load<ShopData>(parts[8].Trim());
                
            if (!result.ContainsKey(dialogueID))
                result[dialogueID] = new List<(int, DialogueLine, string)>();

            result[dialogueID].Add((autoIndex, dialogueLine, rawIndex));
            
            // ★ 수정: 해당 DialogueID의 카운터를 1 증가시킵니다.
            autoIndexCounters[dialogueID]++;
        }
    }
    return result;
}
    
    // SplitCsvLine, SaveAssetToFile 함수는 수정할 필요 없습니다.
    private string[] SplitCsvLine(string line)
    {
        List<string> tokens = new();
        bool inQuotes = false;
        string current = "";

        foreach (char c in line)
        {
            if (c == '"') inQuotes = !inQuotes;
            else if (c == ',' && !inQuotes)
            {
                tokens.Add(current);
                current = "";
            }
            else current += c;
        }
        tokens.Add(current);
        return tokens.ToArray();
    }

#if UNITY_EDITOR
    private void SaveAssetToFile(DialogueAsset asset, string npcId, string suffix)
    {
        string folderPath = "Assets/Resources/GeneratedDialogues";
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets/Resources", "GeneratedDialogues");

        string path = $"{folderPath}/{npcId}_{suffix}.asset";
        AssetDatabase.CreateAsset(asset, path);
        EditorUtility.SetDirty(asset);
    }
#endif
}