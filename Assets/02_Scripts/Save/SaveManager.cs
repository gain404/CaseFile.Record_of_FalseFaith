using System;
using System.Collections;
using System.IO;
using DG.Tweening.Core.Easing;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

/// <summary>
/// 게임의 세이브&로드 시스템을 관리하는 매니저 클래스
/// 3개의 세이브 슬롯을 지원하며, JSON 형태로 데이터를 저장
/// 싱글톤 패턴을 사용하여 전체 게임에서 하나의 인스턴스만 존재
/// </summary>
public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;
    // 최대 세이브 슬롯 개수 (0, 1, 2번 슬롯)
    private const int MAX_SAVE_SLOTS = 3;
    // 세이브 파일이 저장될 디렉토리 경로
    // Application.persistentDataPath + "/SaveFiles"
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
        // 슬롯 인덱스 유효성 검사
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
            // 저장 중 오류 발생 시 로그 출력
            Debug.LogError($"슬롯 {slotIndex} 저장 실패: " + e.Message);
        }
    }

    // 특정 슬롯에서 게임 데이터 로드
    public SaveData LoadGame(int slotIndex)
    {
        // 슬롯 인덱스 유효성 검사
        if (slotIndex < 0 || slotIndex >= MAX_SAVE_SLOTS)
        {
            Debug.LogError("잘못된 슬롯 인덱스: " + slotIndex);
            return new SaveData();
        }

        try
        {
            // 로드할 파일의 전체 경로 생성
            string filePath = Path.Combine(saveDirectory, $"save_slot_{slotIndex}.json");
            // 파일이 존재하는지 확인
            if (File.Exists(filePath))
            {
                // 파일에서 JSON 데이터 읽기
                string jsonData = File.ReadAllText(filePath);
                // JSON을 SaveData로 변환
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
        catch (Exception e)
        {
            Debug.LogError($"슬롯 {slotIndex} 로드 실패: " + e.Message);
            return new SaveData();// 빈 데이터 반환
        }
    }

    // 특정 슬롯에 세이브 파일이 있는지 확인해서 bool값으로 반환
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

    // 모든 세이브 슬롯의 정보를 배열로 반환
    // UI에서 세이브 슬롯 목록을 표시할 때 사용
    public SaveData[] GetAllSaveSlots()
    {
        // 최대 슬롯 개수만큼 배열 생성
        SaveData[] saveSlots = new SaveData[MAX_SAVE_SLOTS];
        // 각 슬롯을 순회하며 데이터 로드
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
        // 슬롯에서 세이브 데이터 로드
        SaveData saveData = LoadGame(slotIndex);
        // 로드된 데이터가 유효하고 씬 이름이 있는지 확인
        if (saveData != null && !string.IsNullOrEmpty(saveData.sceneName))
        {
            // 코루틴으로 안전한 씬 로드 및 데이터 복원
            StartCoroutine(LoadSceneAndRestoreData(saveData));
        }
        else
        {
            Debug.LogError("로드할 데이터가 없거나 씬 이름이 비어있습니다.");
        }
    }

    // 안전한 씬 로드 및 데이터 복원 코루틴
    private IEnumerator LoadSceneAndRestoreData(SaveData saveData)
    {
        // 1. EventSystem 정리 (씬 로드 전)
        // 씬 전환 시 EventSystem이 중복 생성되는 문제 방지
        CleanupEventSystems();

        // 2. 비동기 씬 로드
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(saveData.sceneName);
        yield return asyncLoad;

        // 3. 씬 로드 완료 대기
        yield return new WaitForEndOfFrame();

        // 4. EventSystem 재확인 및 정리
        // 새 씬에서도 EventSystem 중복 문제 해결
        CheckEventSystem();

        // 5. 모든 매니저들이 초기화될 시간 확보
        // Awake, Start 메서드들이 완전히 실행되도록 대기
        yield return new WaitForSeconds(0.1f);

        // 6. 데이터 복원 시작
        yield return StartCoroutine(RestoreGameData(saveData));

        Debug.Log("게임 로드 및 데이터 복원 완료!");
    }

    // EventSystem 정리
    private void CleanupEventSystems()
    {
        // 현재 씬의 모든 EventSystem 찾기
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);
        // 첫 번째를 제외하고 나머지 모두 삭제
        for (int i = 1; i < eventSystems.Length; i++)
        {
            if (eventSystems[i] != null)
            {
                DestroyImmediate(eventSystems[i].gameObject);
            }
        }
    }

    // EventSystem 확인 및 생성
    private void CheckEventSystem()
    {
        // 현재 씬의 모든 EventSystem 찾기
        EventSystem[] eventSystems = FindObjectsByType<EventSystem>(FindObjectsSortMode.None);

        if (eventSystems.Length > 1)
        {
            // 중복 제거
            for (int i = 1; i < eventSystems.Length; i++)
            {
                if (eventSystems[i] != null)
                {
                    DestroyImmediate(eventSystems[i].gameObject);
                }
            }
        }
        else if (eventSystems.Length == 0)
        {
            // EventSystem이 없으면 생성
            GameObject eventSystemGO = new GameObject("EventSystem");
            eventSystemGO.AddComponent<EventSystem>();
            eventSystemGO.AddComponent<StandaloneInputModule>();
            Debug.Log("EventSystem이 생성되었습니다.");
        }
    }

    // 게임 데이터 복원 (순서 중요)
    private IEnumerator RestoreGameData(SaveData saveData)
    {
        //// 1. PlayerManager 찾기 및 대기
        //PlayerManager playerManager = null;
        //float timeout = 3f; // 3초 타임아웃
        //float timer = 0f;

        //while (playerManager == null && timer < timeout)
        //{
        //    playerManager = PlayerManager.Instance;
        //    if (playerManager == null)
        //    {
        //        yield return new WaitForSeconds(0.1f);
        //        timer += 0.1f;
        //    }
        //}

        //if (playerManager == null)
        //{
        //    Debug.LogError("PlayerManager를 찾을 수 없습니다!");
        //    yield break;
        //}

        // 2. 플레이어 오브젝트 찾기
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogError("플레이어 오브젝트를 찾을 수 없습니다!");
            yield break;
        }

        // 3. 플레이어 물리 초기화
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.linearVelocity = Vector2.zero;
            playerRb.angularVelocity = 0f;
        }

        // 4. 플레이어 위치 복원
        // 세이브된 좌표로 플레이어 이동
        Vector3 savedPosition = new Vector3(saveData.posX, saveData.posY, saveData.posZ);
        player.transform.position = savedPosition;

        // 5. 카메라 위치 즉시 동기화
        yield return new WaitForFixedUpdate();
        SyncCameraToPlayer(savedPosition);

        // 6. 플레이어 컨트롤러 상태 복원
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            // 플레이어 활성화 및 이동 가능하게 설정
            playerController.gameObject.SetActive(true);

            Debug.Log($"플레이어 컨트롤러 상태 복원 완료");
        }

        // 7. UI 업데이트 (다음 프레임에)
        yield return new WaitForEndOfFrame();

        // UI 업데이트는 안전하게 별도 메서드에서 처리
        SafeRefreshAllUI();

        // 8. 기타 매니저들 데이터 복원도 메서드로 처리
        SafeRestoreOtherManagersData(saveData);

        Debug.Log($"플레이어 위치 복원 완료: {savedPosition}");
    }

    // 카메라를 플레이어 위치에 동기화
    private void SyncCameraToPlayer(Vector3 playerPosition)
    {
        try
        {
            // Cinemachine Virtual Camera 찾기
            CinemachineCamera virtualCamera =
                FindAnyObjectByType<CinemachineCamera>();

            if (virtualCamera != null)
            {
                // Follow 타겟이 플레이어로 설정되어 있다면, 카메라가 자동으로 따라갈 것임
                // 하지만 즉시 위치를 동기화하려면 강제로 카메라를 이동
                virtualCamera.ForceCameraPosition(playerPosition, Quaternion.identity);
                Debug.Log($"Cinemachine 카메라 위치 동기화: {playerPosition}");
            }
            else
            {
                // Cinemachine이 없는 경우 일반 카메라 처리
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    // 일반 카메라 컨트롤러가 있는 경우
                    PlayerCameraController cameraController = mainCamera.GetComponent<PlayerCameraController>();
                    if (cameraController != null)
                    {
                        cameraController.SetPosition(playerPosition);
                    }
                    else
                    {
                        // 카메라를 플레이어 위치로 즉시 이동 (Z값은 유지)
                        Vector3 cameraPos = mainCamera.transform.position;
                        cameraPos.x = playerPosition.x;
                        cameraPos.y = playerPosition.y;
                        mainCamera.transform.position = cameraPos;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"카메라 동기화 중 오류: {e.Message}");
        }
    }

    // 모든 UI 새로고침
    private void SafeRefreshAllUI()
    {
        try
        {
            // UIHealth 업데이트
            UIHealth[] healthUIs = FindObjectsByType<UIHealth>(FindObjectsSortMode.None);
            foreach (var healthUI in healthUIs)
            {
                if (healthUI != null)
                {
                    healthUI.UpdateHeart();
                }
            }

            // UIManager가 있다면 사용
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UIHealth.UpdateHeart();
            }

            Debug.Log("UI 새로고침 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"UI 새로고침 중 오류: {e.Message}");
        }
    }

    // 기타 매니저들 데이터 복원
    private void SafeRestoreOtherManagersData(SaveData saveData)
    {
        try
        {

            // InventoryManager 데이터 복원
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.SetInventoryData(saveData.inventoryItems);
                UIManager.Instance.UIInventory.RefreshUI();
            }

            if (ObjectiveManager.Instance != null)
            {
                Debug.Log("목표 데이터 불러오기 시도");
                ObjectiveManager.Instance.LoadObjectiveProgress(saveData.activeObjectives, saveData.completedObjectives);
            }



            Debug.Log("기타 매니저 데이터 복원 완료");
        }
        catch (Exception e)
        {
            Debug.LogError($"기타 매니저 데이터 복원 중 오류: {e.Message}");
        }
    }

}