using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class CutSceneSignalTrigger : MonoBehaviour
{
    [SerializeField] private GameObject npcGameObject;
    [SerializeField] private PlayableDirector playableDirector;
    [SerializeField] private LayerMask playerLayerMask;
    [SerializeField] private EnemyController enemyController;
    private Player _player;
    private NPCInteraction _npc;
    
    private void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _player = player.GetComponent<Player>();
        _npc = GetComponent<NPCInteraction>();
        enemyController.DieAction += OnNpc;
    }

    private void Start()
    {
        gameObject.SetActive(false);
        
    }

    private void OnDestroy()
    {
        enemyController.DieAction -= OnNpc;
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
    }
    
    public void OnExitDialogue()
    {
        AfterDialogue();
        gameObject.SetActive(false);
    }

    public void OnExitScene()
    {
        AfterDialogue();
        SceneManager.LoadScene("Chapter1");
    }
    
}
