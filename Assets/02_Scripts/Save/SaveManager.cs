using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    // 현재 플레이 세션에서 참조할 런타임 캐시
    public SaveData ActiveData { get; private set; }

    private const int MAX_SAVE_SLOTS = 3;
    private string saveDirectory;

    // LoadingBar 경유 로드시 복원을 위해 보관할 값들
    private SaveData _pendingRestoreData;
    private string   _pendingTargetScene;
    private bool     _loadingViaLoadingBar;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            if (ActiveData == null)
                ActiveData = new SaveData();

            saveDirectory = Path.Combine(Application.persistentDataPath, "SaveFiles");
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);

            // 타겟 씬 로드 직후 자동 복원용 콜백
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log($"세이브 디렉토리: {saveDirectory}");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // ---------------- 저장/로드 ----------------

    public void SaveGame(SaveData data, int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스");
            return;
        }

        try
        {
            // 문 해제/컷씬 진행 목록을 ActiveData에서 복사 포함
            if (ActiveData != null)
            {
                if (ActiveData.unlockedPassages == null)
                    ActiveData.unlockedPassages = new List<string>();
                if (data.unlockedPassages == null)
                    data.unlockedPassages = new List<string>();
                data.unlockedPassages.Clear();
                data.unlockedPassages.AddRange(ActiveData.unlockedPassages);

                // (있다면) 컷씬 목록도 동기화
                if (ActiveData.playedCutscenes == null)
                    ActiveData.playedCutscenes = new List<string>();
                if (data.playedCutscenes == null)
                    data.playedCutscenes = new List<string>();
                data.playedCutscenes.Clear();
                data.playedCutscenes.AddRange(ActiveData.playedCutscenes);
            }

            data.saveTime = DateTime.Now;

            string jsonData = JsonUtility.ToJson(data, true);
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");
            File.WriteAllText(filePath, jsonData);

            Debug.Log($"슬롯 {slotIndex} 저장 완료\n{jsonData}");
        }
        catch (Exception e)
        {
            Debug.LogError($"저장 실패: {e.Message}");
        }
    }

    public SaveData LoadGame(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스");
            return new SaveData();
        }

        try
        {
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");
            if (File.Exists(filePath))
            {
                string jsonData = File.ReadAllText(filePath);
                var data = JsonUtility.FromJson<SaveData>(jsonData);
                Debug.Log($"슬롯 {slotIndex} 로드 완료\n{jsonData}");
                return data;
            }
            else
            {
                Debug.Log($"슬롯 {slotIndex} 세이브 파일 없음");
                return new SaveData();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"로드 실패: {e.Message}");
            return new SaveData();
        }
    }

    public bool HasSaveFile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS) return false;
        string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");
        return File.Exists(filePath);
    }

    public void DeleteSaveFile(int slotIndex)
    {
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스");
            return;
        }

        try
        {
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                Debug.Log($"슬롯 {slotIndex} 삭제 완료");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"삭제 실패: {e.Message}");
        }
    }

    public SaveData[] GetAllSaveSlots()
    {
        var saveSlots = new SaveData[MAX_SAVE_SLOTS];
        for (int i = 0; i < MAX_SAVE_SLOTS; i++)
            saveSlots[i] = HasSaveFile(i) ? LoadGame(i) : null;
        return saveSlots;
    }
    
    public void LoadGameAndScene(int slotIndex)
    {
        var saveData = LoadGame(slotIndex);

        // 런타임 캐시 갱신
        ActiveData = saveData ?? new SaveData();

        if (saveData != null && !string.IsNullOrEmpty(saveData.sceneName))
        {
            // 로딩바 경유 복원 예약
            _pendingRestoreData   = ActiveData;
            _pendingTargetScene   = ActiveData.sceneName;
            _loadingViaLoadingBar = true;

            // 로딩 씬 → 타겟 씬 로드
            LoadingBar.LoadScene(_pendingTargetScene);
        }
        else
        {
            Debug.LogError("로드할 데이터가 없거나 씬 이름이 비어있음");
        }
    }

    // ---------------- LoadingBar 경유 복원 ----------------

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 로딩바 경유가 아니거나, 다른 씬이면 무시
        if (!_loadingViaLoadingBar) return;
        if (!string.IsNullOrEmpty(_pendingTargetScene) && scene.name != _pendingTargetScene) return;

        StartCoroutine(RestoreAfterLoadingBar());
    }

    private IEnumerator RestoreAfterLoadingBar()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(0.1f);

        if (_pendingRestoreData != null)
            yield return StartCoroutine(RestoreGameData(_pendingRestoreData));

        _pendingRestoreData   = null;
        _pendingTargetScene   = null;
        _loadingViaLoadingBar = false;
    }

    // ---------------- 복원 루틴/헬퍼 ----------------

    private IEnumerator RestoreGameData(SaveData saveData)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("플레이어 오브젝트 없음");
            yield break;
        }

        var rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        var pos = new Vector3(saveData.posX, saveData.posY, saveData.posZ);
        player.transform.position = pos;

        yield return new WaitForFixedUpdate();
        SyncCameraToPlayer(pos);

        var pc = player.GetComponent<PlayerController>();
        if (pc != null)
            pc.gameObject.SetActive(true);

        yield return new WaitForEndOfFrame();
        SafeRefreshAllUI();
        SafeRestoreOtherManagersData(saveData);

        Debug.Log($"플레이어 위치 복원 완료: {pos}");
    }

    private void SyncCameraToPlayer(Vector3 playerPosition)
    {
        try
        {
            var vcam = FindAnyObjectByType<CinemachineCamera>();
            if (vcam != null)
            {
                vcam.ForceCameraPosition(playerPosition, Quaternion.identity);
            }
            else
            {
                var cam = Camera.main;
                if (cam != null)
                {
                    var ctrl = cam.GetComponent<PlayerCameraController>();
                    if (ctrl != null)
                        ctrl.SetPosition(playerPosition);
                    else
                    {
                        var p = cam.transform.position;
                        p.x = playerPosition.x; p.y = playerPosition.y;
                        cam.transform.position = p;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"카메라 동기화 오류: {e.Message}");
        }
    }

    private void SafeRefreshAllUI()
    {
        try
        {
            var healthUIs = FindObjectsByType<UIHealth>(FindObjectsSortMode.None);
            foreach (var h in healthUIs) h?.UpdateHeart();

            if (UIManager.Instance != null)
                UIManager.Instance.UIHealth.UpdateHeart();
        }
        catch (Exception e)
        {
            Debug.LogError($"UI 새로고침 오류: {e.Message}");
        }
    }

    private void SafeRestoreOtherManagersData(SaveData saveData)
    {
        try
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SetInventoryData(saveData.inventoryItems);
                UIManager.Instance.UIInventory.RefreshUI();
            }

            if (ObjectiveManager.Instance != null)
            {
                ObjectiveManager.Instance.LoadObjectiveProgress(
                    saveData.activeObjectives,
                    saveData.completedObjectives
                );
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"기타 매니저 복원 오류: {e.Message}");
        }
    }

    // 현재 게임 상태를 캡처해 SaveData 생성 (세이브 버튼에서 사용)
    public SaveData BuildCurrentSaveData()
    {
        var data = new SaveData();
        data.sceneName = SceneManager.GetActiveScene().name;

        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var p = player.transform.position;
            data.posX = p.x; data.posY = p.y; data.posZ = p.z;
        }

        if (InventoryManager.Instance != null)
            data.inventoryItems = InventoryManager.Instance.GetInventoryData();

        if (ObjectiveManager.Instance != null)
        {
            data.activeObjectives   = ObjectiveManager.Instance.GetActiveObjectives();
            data.completedObjectives = new List<ObjectiveData>(ObjectiveManager.Instance.GetCompletedObjectives());
        }

        if (ActiveData?.unlockedPassages != null)
            data.unlockedPassages = new List<string>(ActiveData.unlockedPassages);

        if (ActiveData?.playedCutscenes != null)
            data.playedCutscenes = new List<string>(ActiveData.playedCutscenes);

        return data;
    }
}
