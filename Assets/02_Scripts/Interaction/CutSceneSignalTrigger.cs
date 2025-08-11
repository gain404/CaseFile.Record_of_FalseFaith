using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutSceneSignalTrigger : MonoBehaviour
{
    public LayerMask playerLayerMask;
    [SerializeField] private GameObject npcGameObject;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private EnemyController enemyController;
    private Player _player;
    private NPCInteraction _npc;
    private UIManager _uiManager;
    private UIFadePanel _uiFadePanel;
    
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
        _npc = GetComponent<NPCInteraction>();
        if (enemyController != null)
        {
            enemyController.DieAction += OnNpc;
        }
    }

    private void Start()
    {
        _uiFadePanel = UIManager.Instance.UIFadePanel;
    }

    private void OnDestroy()
    {
        if (enemyController != null)
        {
            enemyController.DieAction -= OnNpc;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & playerLayerMask) != 0)
        {
            playableDirector.Play();
        }
    }
    
    public void OffInput()
    {
        _player.PlayerController.playerActions.Disable();
    }

    private void OnNpc()
    {
        gameObject.SetActive(true);
    }
    
    public void OnEnterDialogue()
    {
        //기존의 도하 꺼 주기
        if (npcGameObject != null)
        {
            npcGameObject.SetActive(false);
        }
        OnNpc();
        _player.CurrentInteractableNPC = _npc;
        UIManager.Instance.UIDialogue.StartDialogue(_npc.GetFirstDialogue(), _npc.transform);
        UIManager.Instance.UIDialogue.autoAdvanced = true;
    }

    private void AfterDialogue()
    {
        _player.PlayerController.playerActions.Enable();
        _player.CurrentInteractableNPC = null;
        UIManager.Instance.UIDialogue.autoAdvanced = false;
        npcGameObject.SetActive(true);
    }
    
    public void OnExitDialogue()
    {
        AfterDialogue();
        gameObject.SetActive(false);
    }

    public void OnExitScene()
    {
        AfterDialogue();
        StartCoroutine(FadeAndLoadScene("Chapter1"));
    }
    
    private IEnumerator FadeAndLoadScene(string sceneName)
    {
        
        _uiFadePanel.Fade(1f, 3f);
        yield return new WaitForSeconds(3.5f);
        // 로딩 씬 호출
        LoadingBar.LoadScene(sceneName);
    }

    public void OnMoveTutorialPanel()
    {
        DOVirtual.DelayedCall(0.1f, () => UIManager.Instance.UITutorial.OnTutorialPanel(TutorialType.Move));
    }
    public void OnInvestigationTutorialPanel()
    {
        DOVirtual.DelayedCall(0.1f, () => UIManager.Instance.UITutorial.OnTutorialPanel(TutorialType.Move));
    }

    public void OffUI()
    {
        if (_uiManager.MapManager != null)
        {
            _uiManager.MapManager.OnMapButton();
        }
        _uiManager.UIInvestigation.OnBookButton();
        _uiManager.UIHealth.OnHealthUI();
        _uiManager.UIStamina.OnStaminaUI();
    }
}
