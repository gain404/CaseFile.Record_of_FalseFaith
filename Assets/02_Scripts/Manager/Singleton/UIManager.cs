using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public UIInventory UIInventory { get; private set; }
    public UIDialogue UIDialogue { get; private set; }

    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private List<UIEntry> uiPrefabs;

    private Dictionary<UIType, GameObject> _activeUIs = new();
    private GameObject _canvas;

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //이거 awake이후에 실행되므로 ui캐싱은 Start에서 실행시켜주기
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        InitSceneUI(scene.name); // 자동 호출
    }
    
    private void InitSceneUI(string scene)
    {
        _canvas = Instantiate(canvasPrefab);
        ClearUI();
        foreach (UIEntry uiEntry in uiPrefabs)
        {
            if (uiEntry.sceneName == scene)
            {
                ShowUI(uiEntry.uiType);
            }
        }

        if (_activeUIs.TryGetValue(UIType.UIInventory, out GameObject ui))
        {
            UIInventory = ui.GetComponent<UIInventory>();
        }
    }
    
    //canvas를 생성하고 씬에 맞는 ui생성
    private void ShowUI(UIType uiName)
    {
        if (_activeUIs.ContainsKey(uiName)) return;

        GameObject prefab = uiPrefabs.Find(x => x.uiType == uiName).uiPrefab;
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, _canvas.transform);
            if (instance != null)
            {
                _activeUIs.Add(uiName, instance);
            }
        }
    }

    //dictionary초기화할때 사용
    public void HideUI(UIType uiName)
    {
        if (_activeUIs.TryGetValue(uiName, out GameObject ui))
        {
            Destroy(ui);
            _activeUIs.Remove(uiName);
        }
    }

    public void ClearUI()
    {
        _activeUIs.Clear();
    }

    public GameObject GetUI(UIType uiName)
    {
        return _activeUIs.GetValueOrDefault(uiName);
    }
}
