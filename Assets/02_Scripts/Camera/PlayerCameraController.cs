using System;
using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerCameraController : MonoBehaviour
{
    public bool isRunning;
    
    [SerializeField] private float defaultZoomSize = 10f; //기본 카메라
    [SerializeField] private float maxZoomOutSize = 15f; //달릴 때 카메라
    [SerializeField] private float zoomSpeed = 5f; //얼마나 빨리 변할지

    private float _currentDefaultZoom;
    private float _currentMaxZoom;
    private CinemachineCamera _playerCamera;
    private Coroutine _zoomCoroutine;

    private void Awake()
    {
        _playerCamera = GetComponent<CinemachineCamera>();
        _currentDefaultZoom = defaultZoomSize;
        _currentMaxZoom = maxZoomOutSize;
        isRunning = false;
    }

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerCamera.Follow = player.transform.Find("CameraTarget");
    }

    public void SetZoomLimits(float newDefault, float newMax)
    {
        _currentDefaultZoom = newDefault;
        _currentMaxZoom = newMax;

        // 현재 줌 상태에 따라 재적용
        if (isRunning)
            ZoomOutForRunning();
        else
            ZoomInToDefault();
    }

    public void ResetZoomLimits()
    {
        _currentDefaultZoom = defaultZoomSize;
        _currentMaxZoom = maxZoomOutSize;

        // 현재 줌 상태에 따라 재적용
        if (isRunning)
            ZoomOutForRunning();
        else
            ZoomInToDefault();
    }

    public void ZoomOutForRunning()
    {
        float target = _currentMaxZoom;
        StartZoom(target);
    }

    public void ZoomInToDefault()
    {
        StartZoom(_currentDefaultZoom);
    }

    private void StartZoom(float targetSize)
    {
        if (_zoomCoroutine != null)
        {
            StopCoroutine(_zoomCoroutine);
            _zoomCoroutine = null;
        }
        _zoomCoroutine = StartCoroutine(ZoomTo(targetSize));
    }

    private IEnumerator ZoomTo(float targetSize)
    {
        float initialSize = _playerCamera.Lens.OrthographicSize;
        Vector3 initialPosition = _playerCamera.transform.position;

        while (Mathf.Abs(_playerCamera.Lens.OrthographicSize - targetSize) > 0.01f)
        {
            float currentSize = _playerCamera.Lens.OrthographicSize;
            currentSize = Mathf.Lerp(currentSize, targetSize, Time.deltaTime * zoomSpeed);
            _playerCamera.Lens.OrthographicSize = currentSize;

            // 중심 기준 보정 (줌 시 아래로 내리기)
            float offsetRatio = (currentSize - initialSize) / initialSize;
            Vector3 offset = Vector3.down * offsetRatio;

            _playerCamera.transform.position = initialPosition + offset;

            yield return null;
        }

        _playerCamera.Lens.OrthographicSize = targetSize;
    }

    public void DisableZoomTemporarily()
    {
        _currentDefaultZoom = defaultZoomSize;
        _currentMaxZoom = defaultZoomSize;
    }


    //테스트용
    [Header("Boundary Settings")]
    public bool useBoundary = false;
    public float minX, maxX, minY, maxY; // 카메라 이동 제한
    public Vector3 offset = new Vector3(0, 0, -10); // 카메라 오프셋

    // 세이브/로드 시 카메라 위치를 즉시 설정하는 메서드
    public void SetPosition(Vector3 playerPosition)
    {
        Vector3 newPosition = playerPosition + offset;

        // 경계 제한 적용
        if (useBoundary)
        {
            newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
            newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        }

        // 즉시 위치 설정 (부드러운 이동 없이)
        transform.position = newPosition;

        Debug.Log($"카메라 위치 즉시 설정: {newPosition}");
    }
}
