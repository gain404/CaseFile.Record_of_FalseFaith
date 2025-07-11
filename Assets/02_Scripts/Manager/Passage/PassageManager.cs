using System;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class PassageManager : MonoBehaviour
{
    public static PassageManager Instance { get; private set; }

    public bool canMovement;
    
    private GameObject _player;
    private GameObject _playerCamera;
    private bool _isSceneChange;
    private string _targetScene;
    private Vector3 _targetPosition;
    private PlayerController _playerController;
    private CinemachineCamera _playerCinemachineCamera;
    private CinemachineConfiner2D _playerCinemachineConfiner2D;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        _player = GameObject.FindWithTag("Player");
        _playerCamera = GameObject.FindWithTag("PlayerCamera");
        _playerController = _player.GetComponent<PlayerController>();
        _playerCinemachineCamera = _playerCamera.GetComponent<CinemachineCamera>();
        _playerCinemachineConfiner2D = _playerCamera.GetComponent<CinemachineConfiner2D>();
        canMovement = false;
    }

    private void Update()
    {
        if (canMovement)
        {
            if (_playerController.playerActions.Interact.WasPerformedThisFrame())
            {
                StartPassage();
            }
        }
    }

    public void SetInfo(bool isSceneChange, string targetScene, Vector3 targetPosition)
    {
        _isSceneChange = isSceneChange;
        _targetScene = targetScene;
        _targetPosition = targetPosition;

    }

    private void StartPassage()
    {
        //페이드 아웃
        if (_isSceneChange)
        {
            //씬 어둡게
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(_targetScene);
        }
        else
        {
            SetPlayerPosition();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        _player = GameObject.FindWithTag("Player");
        _playerCamera = GameObject.FindWithTag("PlayerCamera");
        _playerController = _player.GetComponent<PlayerController>();
        _playerCinemachineCamera = _playerCamera.GetComponent<CinemachineCamera>();
        _playerCinemachineConfiner2D = _playerCamera.GetComponent<CinemachineConfiner2D>();
        _playerCinemachineCamera.Follow = _player.transform;
        SetPlayerPosition();
        
        SceneManager.sceneLoaded -= OnSceneLoaded; 
    }

    private void SetPlayerPosition()
    {
        Quaternion quaternion = new Quaternion(0, 0, 0, 0);
        _playerCinemachineConfiner2D.enabled = false;
        _player.transform.position = _targetPosition;
        _playerCinemachineCamera.ForceCameraPosition(_targetPosition, quaternion);
        DOVirtual.DelayedCall(1,cameraColliderOn);
        
        canMovement = false;
    }

    private void cameraColliderOn()
    {
        _playerCinemachineConfiner2D.enabled = true;
    }
}
