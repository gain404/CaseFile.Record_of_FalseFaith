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
        // 페이드 아웃
        FadeManager.Instance.Fade(1f, 4f);
        yield return new WaitForSeconds(1);
        // 로딩 씬 호출
        LoadingBar.LoadScene(sceneName);
    }

    public void OnMoveTutorialPanel()
    {
        DOVirtual.DelayedCall(0.1f, () => UIManager.Instance.UITutorial.OnTutorialPanel(TutorialType.Move));
    }
    
}
