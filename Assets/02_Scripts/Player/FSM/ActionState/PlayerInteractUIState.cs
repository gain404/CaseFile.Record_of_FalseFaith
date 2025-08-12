using UnityEngine;

public class PlayerInteractUIState : PlayerActionState
{
    public PlayerInteractUIState(PlayerStateMachine playerStateMachine) : base(playerStateMachine) { }

    public override void Enter()
    {
        Debug.Log("--- 상점/UI 상태 진입 ---");
        base.Enter();
        stateMachine.MovementSpeedModifier = 0f;
    }

    public override void Update()
    {
    }
    
    public override void PhysicsUpdate()
    {
    }

    public override void Exit()
    {
        Debug.Log("--- 상점/UI 상태 퇴장 ---");
        base.Exit();
        stateMachine.MovementSpeedModifier = 1f;

        //상점/조사를 닫고 나가는 것이라면, 모든 것을 여기서 정리
        if (stateMachine.IsReturningFromShop || stateMachine.IsReturningFromInvestigationCancel)
        {
            // UIDialogue의 카메라와 UI를 초기화
            UIManager.Instance.UIDialogue.ResetDialogueState();

            // 사용했던 플래그를 다시 false로 설정
            stateMachine.IsReturningFromShop = false;
            stateMachine.IsReturningFromInvestigationCancel = false;
        }
    }
}