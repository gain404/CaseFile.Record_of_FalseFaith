using UnityEngine;

public class PlayerChatState : PlayerActionState
{
    
    public PlayerChatState(PlayerStateMachine playerStateMachine) : base(playerStateMachine)
    {
    }

    //상태 진입할 때
    public override void Enter()
    {
        base.Enter();
        stateMachine.MovementSpeedModifier = 0f; //조사 중 움직임X
        //DialogueManager.Instance.StartChat <<이런 식으로 Chat 시작
        StartAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }

    //상태 빠져나올 때
    public override void Exit()
    {
        base.Exit();
        stateMachine.MovementSpeedModifier = moveData.WalkSpeedModifier;
        //DialogueManager.Instance.EndChat <<상황 종료 됐을 때
        EndAnimation(stateMachine.Player.PlayerAnimationData.IdleParameterHash);
    }
}
