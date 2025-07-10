using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassageManager : MonoBehaviour
{
    public static PassageManager Instance { get; private set; }

    private GameObject _player;
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        _player = GameObject.FindWithTag("Player");
    }

    public void StartPassage(bool isSceneChange, string targetScene, Vector3 targetPosition)
    {
        //페이드 아웃
        if (isSceneChange)
        {
            SceneManager.LoadScene(targetScene);
            //씬 어둡게
            _player = GameObject.FindWithTag("Player");
        }
        //카메라 이동이 이상하면 여기에 추가
        _player.transform.position = targetPosition;
    }
}
