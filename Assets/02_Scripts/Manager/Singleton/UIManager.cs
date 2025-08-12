using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : Singleton<UIManager>
{
    public UIInventory UIInventory { get; private set; }
    public UIDialogue UIDialogue { get; private set; }
    public UIShop UIShop { get; private set; }
    public UIHealth UIHealth { get; private set; }
    public UIStamina UIStamina { get; private set; }

    public UISave UISave { get; private set; }
    public UILoad UILoad { get; private set; }
    public UIInvestigation UIInvestigation { get; private set; }
    public UIGuideIcon UIGuideIcon { get; private set; }
    public UITutorial UITutorial { get; private set; }
    public UIMap UIMap { get; private set; }
    public UIInvestigationTimer UIInvestigationTimer { get; private set; }
    public UIFadePanel UIFadePanel { get; private set; }
    public UIObjective UIObjective { get; private set; }
    public UIObjectiveCompleteNotifier UIObjectiveCompleteNotifier { get; private set; }

    public UIGameOverEffect UIGameOverEffect { get; private set; }
    
    public UIEndingPanel UIEndingPanel { get; private set; }

    [SerializeField] private GameObject canvasPrefab;
    [SerializeField] private List<UIEntry> uiPrefabs;

    private Dictionary<UIType, GameObject> _activeUIs = new();
    private GameObject _canvas;
    protected override void Awake()
    {
        base.Awake();
        InitSceneUI(SceneManager.GetActiveScene().name);
    }

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
        // 기존 Canvas가 있다면 제거 - 씬 로드할 때 중복 방지
        if (_canvas != null)
        {
            Destroy(_canvas);
            _canvas = null;
        }

        _canvas = Instantiate(canvasPrefab);
        ClearUI();
        foreach (UIEntry uiEntry in uiPrefabs)
        {
            foreach (SceneName sceneName  in uiEntry.sceneName)
            {
                if (sceneName.ToString() == scene)
                {
                    ShowUI(uiEntry.uiType);
                    break;
                }
            }
        }

        UIInventory = GetUIComponent<UIInventory>(UIType.UIInventory);
        UIDialogue = GetUIComponent<UIDialogue>(UIType.UIDialogue);
        UIShop = GetUIComponent<UIShop>(UIType.UIShop);
        UIHealth = GetUIComponent<UIHealth>(UIType.UIHealth);
        UIStamina = GetUIComponent<UIStamina>(UIType.UIStamina);
        UISave = GetUIComponent<UISave>(UIType.UISave);
        UILoad = GetUIComponent<UILoad>(UIType.UILoad);
        UIInvestigation = GetUIComponent<UIInvestigation>(UIType.UIInvestigation);
        UIGuideIcon = GetUIComponent<UIGuideIcon>(UIType.UIGuideIcon);
        UITutorial = GetUIComponent<UITutorial>(UIType.UITutorial);
        UIInvestigationTimer = GetUIComponent<UIInvestigationTimer>(UIType.UIInvestigationTimer);
        UIObjective = GetUIComponent<UIObjective>(UIType.UIObjective);
        UIObjectiveCompleteNotifier = GetUIComponent<UIObjectiveCompleteNotifier>(UIType.UIObjectiveCompleteNotifier);
        UIMap = GetUIComponent<UIMap>(UIType.UIMap);
        UIFadePanel = GetUIComponent<UIFadePanel>(UIType.UIFadePanel);
        UIEndingPanel = GetUIComponent<UIEndingPanel>(UIType.UIEndingPanel);
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
    
    private T GetUIComponent<T>(UIType uiType) where T : Component
    {
        if (_activeUIs.TryGetValue(uiType, out GameObject ui))
        {
            return ui.GetComponent<T>();
        }
        return null;
    }
}
