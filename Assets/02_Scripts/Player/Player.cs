using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("Animation")] [field: SerializeField]
    public PlayerAnimationData PlayerAnimationData { get; private set; }
    [field : SerializeField] public CharacterData Data { get; private set; }
    
    public Animator Animator { get; private set; }
    public PlayerController PlayerController { get; private set; }
    public CharacterController CharacterController { get; private set; }
    public SpriteRenderer PlayerSpriteRenderer { get; private set; }
    private PlayerStateMachine _stateMachine;

    public ItemData itemData;//추가한 스크립트(송도현)
    public Action addItem;//추가한 스크립트(송도현)

    public GameObject talkBalloon;//추가한 스크립트(송도현)
    public NPCInteraction CurrentInteractableNPC { get; set; }
    public ItemInteraction CurrentInteractableItem { get; set; }

    private void Awake()
    {
        // 싱글톤매니저에 Player를 참조할 수 있게 데이터를 넘긴다.
        TestCharacterManager.Instance.Player = this;
        //애니메이션 string -> int로 초기화
        PlayerAnimationData.Initialize();
        
        Animator = GetComponent<Animator>();
        PlayerController = GetComponent<PlayerController>();
        CharacterController = GetComponent<CharacterController>();
        PlayerSpriteRenderer = GetComponent<SpriteRenderer>();
        _stateMachine = new PlayerStateMachine(this);
        
        _stateMachine.ChangeState(_stateMachine.IdleState);

        if(talkBalloon != null)
        {
            talkBalloon.SetActive(false);//추가한 스크립트(송도현)
        }
    }

    private void Update()
    {
        _stateMachine.HandleInput();
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.PhysicsUpdate();
    }
}
