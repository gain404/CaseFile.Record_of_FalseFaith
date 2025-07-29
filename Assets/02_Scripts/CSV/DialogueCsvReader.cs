using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DialogueCsvReader : MonoBehaviour
{
    [Tooltip("Resources 폴더에 있는 대화 CSV 파일을 연결하세요.")]
    [SerializeField] private TextAsset dialogueCSV;

    [Tooltip("프로젝트의 모든 NPCData 에셋들을 여기에 미리 연결해야 합니다.")]
    [SerializeField] private List<NPCData> allNpcData;

    void Awake()
    {
        // 게임이 시작될 때 딱 한 번만 실행되도록 설정
        // (이미 다른 씬에서 실행되었다면 이 오브젝트는 파괴됨)
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
        // 1. CSV 파싱하여 DialogueID 별로 라인들 그룹화
        var parsedDialogues = ParseCsvAndGroupById();

        // 2. NPCData를 ID로 쉽게 찾기 위해 딕셔너리로 변환
        var npcDataMap = allNpcData.ToDictionary(data => data.npcID, data => data);

        // 3. 그룹화된 대화 데이터를 기반으로 DialogueAsset 생성 및 연결
        foreach (var entry in parsedDialogues)
        {
            string dialogueID = entry.Key;
            List<DialogueLine> lines = entry.Value;

            // DialogueAsset을 메모리에 동적으로 생성
            DialogueAsset newAsset = ScriptableObject.CreateInstance<DialogueAsset>();
            newAsset.lines = lines.ToArray();

            // DialogueID를 분석하여 NPC ID와 접미사(_First, _Second) 분리
            int lastUnderscoreIndex = dialogueID.LastIndexOf('_');
            if (lastUnderscoreIndex == -1)
            {
                Debug.LogError($"[DialogueInitializer] DialogueID '{dialogueID}'에 '_First' 또는 '_Second' 접미사가 없어 NPC를 찾을 수 없습니다.");
                continue;
            }

            string npcID = dialogueID.Substring(0, lastUnderscoreIndex);
            string suffix = dialogueID.Substring(lastUnderscoreIndex + 1);

            newAsset.npcID = npcID;

            // 4. 올바른 NPCData를 찾아 DialogueAsset 연결
            if (npcDataMap.TryGetValue(npcID, out NPCData targetNpcData))
            {
                if (suffix.Equals("First", System.StringComparison.OrdinalIgnoreCase))
                {
                    targetNpcData.firstDialogueAsset = newAsset;
                }
                else if (suffix.Equals("Second", System.StringComparison.OrdinalIgnoreCase))
                {
                    targetNpcData.secondDialogueAsset = newAsset;
                }
            }
            else
            {
                Debug.LogWarning($"[DialogueInitializer] CSV에 명시된 Npc ID '{npcID}'에 해당하는 NPCData를 찾을 수 없습니다.");
            }
        }
        
        Debug.Log("CSV로부터 모든 다이얼로그 초기화 완료!");
    }

    // CSV를 파싱하여 Dictionary<DialogueID, List<DialogueLine>> 형태로 반환하는 함수
    private Dictionary<string, List<DialogueLine>> ParseCsvAndGroupById()
    {
        var dialogueDatabase = new Dictionary<string, List<DialogueLine>>();
        
        string[] lines = dialogueCSV.text.Split('\n');

        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] columns = line.Split(',');

            string dialogueID = columns[0];
            if (string.IsNullOrEmpty(dialogueID)) continue;

            DialogueLine dialogueLine = new DialogueLine();
            dialogueLine.type = (DialogueType)System.Enum.Parse(typeof(DialogueType), columns[2]);
            dialogueLine.characterName = columns[3];
            dialogueLine.text = columns[4];

            // Portrait, Choices 등 다른 데이터 파싱 로직 추가 (이전 답변 참고)
            // 예: if (!string.IsNullOrEmpty(columns[5])) dialogueLine.portrait = Resources.Load<Sprite>(columns[5]);
            if (!string.IsNullOrEmpty(columns[8])) dialogueLine.shopData = Resources.Load<ShopData>("ShopData/" + columns[8].Trim());
            
            if (!dialogueDatabase.ContainsKey(dialogueID))
            {
                dialogueDatabase[dialogueID] = new List<DialogueLine>();
            }
            dialogueDatabase[dialogueID].Add(dialogueLine);
        }
        return dialogueDatabase;
    }
}