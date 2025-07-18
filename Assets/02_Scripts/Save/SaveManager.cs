using System;
using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private const int MAX_SAVE_SLOTS = 3; //변경되지 않도록 const로
    private string saveDirectory;

    void Awake()
    {
        // 싱글톤 패턴 적용
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 세이브 파일 경로 설정
            saveDirectory = Path.Combine(Application.persistentDataPath, "SaveFiles");

            // 디렉토리가 없으면 생성
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            Debug.Log("세이브 디렉토리: " + saveDirectory);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 특정 슬롯에 게임 데이터 저장
    public void SaveGame(SaveData data, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스: " + slotIndex);
            return;
        }

        try
        {
            // 현재 시간 저장
            data.saveTime = DateTime.Now;

            // JSON으로 변환
            string jsonData = JsonUtility.ToJson(data, true);

            // 파일 경로 설정
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");

            // 파일에 저장
            File.WriteAllText(filePath, jsonData);

            Debug.Log($"슬롯 {slotIndex}에 게임 저장 완료!");
            Debug.Log("저장된 데이터: " + jsonData);
        }
        catch (Exception e)
        {
            Debug.LogError($"슬롯 {slotIndex} 저장 실패: " + e.Message);
        }
    }


    // 특정 슬롯에서 게임 데이터 로드
    public SaveData LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스: " + slotIndex);
            return new SaveData();
        }
        //try-catch로 오류가 날 경우에 대비
        try//오류가 발생할 수 있는 구문
        {
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");

            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                SaveData data = JsonUtility.FromJson<SaveData>(jsonData);

                Debug.Log($"슬롯 {slotIndex}에서 게임 로드 완료!");
                Debug.Log("로드된 데이터: " + jsonData);

                return data;
            }
            else
            {
                Debug.Log($"슬롯 {slotIndex}에 세이브 파일이 존재하지 않습니다.");
                return new SaveData();
            }
        }
        catch (Exception e)//오류가 발생했을 때 실행시킬 구문
        {
            Debug.LogError($"슬롯 {slotIndex} 로드 실패: " + e.Message);
            return new SaveData();
        }
    }

    // 특정 슬롯에 세이브 파일이 있는지 확인
    public bool HasSaveFile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
            return false;

        string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");
        return File.Exists(filePath);
    }


    // 특정 슬롯의 세이브 파일 삭제
    public void DeleteSaveFile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스: " + slotIndex);
            return;
        }

        try
        {
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"슬롯 {slotIndex} 세이브 파일 삭제 완료!");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"슬롯 {slotIndex} 삭제 실패: " + e.Message);
        }
    }

    // 모든 슬롯 정보 가져오기
    public SaveData[] GetAllSaveSlots()
    {
        SaveData[] saveSlots = new SaveData[MAX_SAVE_SLOTS];

        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
        {
            if (HasSaveFile(i))
            {
                saveSlots[i] = LoadGame(i);
            }
            else
            {
                saveSlots[i] = null; // 빈 슬롯
            }
        }

        return saveSlots;
    }

    // 게임 로드 및 씬 이동
    public void LoadGameAndScene(int slotIndex)
    {
        SaveData saveData = LoadGame(slotIndex);

        if (saveData != null)
        {
            // 씬 이동
            if (!string.IsNullOrEmpty(saveData.sceneName))
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(saveData.sceneName);
            }

            // 플레이어 위치 복원 (씬 로드 후 실행되도록 코루틴 사용)
            StartCoroutine(RestorePlayerPosition(saveData));
        }
    }

    private System.Collections.IEnumerator RestorePlayerPosition(SaveData saveData)
    {
        // 씬 로드 완료까지 대기
        yield return new WaitForEndOfFrame();

        GameObject player = PlayerManager.Instance.GetPlayer();

        if (player != null)
        {
            // 플레이어 위치 복원
            Vector3 savedPosition = new Vector3(saveData.posX, saveData.posY, saveData.posZ);
            player.transform.position = savedPosition;

            // 추가 플레이어 데이터 복원 (필요에 따라 수정)
            /*
            PlayerController playerController = player.GetComponent<PlayerController>();
            if (playerController != null)
            {
                playerController.health = saveData.health;
                playerController.playerName = saveData.playerName;
            }
            */

            Debug.Log($"플레이어 위치 복원 완료: {savedPosition}");
        }
    }
}
