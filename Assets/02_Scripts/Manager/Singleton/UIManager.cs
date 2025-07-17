using System.Collections.Generic;
using UnityEngine;


public class UIManager : Singleton<UIManager>
{
    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private List<UIEntry> uiPrefabs;

    private Dictionary<UIType, Component> _activeUIs = new();
    private GameObject _canvas;

    //씬 시작할때 가장 먼저 호출
    public void InitSceneUI()
    {
        _canvas = Instantiate(canvasPrefab);
    }
    
    //canvas를 생성하고 씬에 맞는 ui생성
    public void ShowUI<T>(UIType uiName) where T : Component
    {
        if (_activeUIs.ContainsKey(uiName)) return;

        GameObject prefab = uiPrefabs.Find(x => x.uiType == uiName).uiPrefab;
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, _canvas.transform);
            T uiComponent = instance.GetComponent<T>();
            if (uiComponent != null)
            {
                _activeUIs.Add(uiName, uiComponent);
            }
        }
    }

    //dictionary초기화할때 사용
    public void HideUI(UIType uiName)
    {
        if (_activeUIs.TryGetValue(uiName, out Component ui))
        {
            Destroy(ui);
            _activeUIs.Remove(uiName);
        }
    }

    public void ClearUI()
    {
        _activeUIs.Clear();
    }

    public T GetUI<T>(UIType uiName) where T : Component
    {
        if (_activeUIs.TryGetValue(uiName, out var component))
        {
            return component as T;
        }
        return null;
    }
}
