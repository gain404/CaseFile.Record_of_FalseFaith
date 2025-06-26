using UnityEngine;
using System;

[Serializable]
public class PlayerAnimationData
{
    //--------------------parameter 정의------------------//
    
    //Move State
    [SerializeField] private string moveParameterName = "@Move";
    [SerializeField] private string idleParameterName = "Idle";
    [SerializeField] private string dashParameterName = "Dash";
    [SerializeField] private string runParameterName = "Run";
    [SerializeField] private string walkParameterName = "Walk";
    
    //Ground State
    [SerializeField] private string groundParameterName = "@Ground";
    [SerializeField] private string jumpParameterName = "Jump";
    [SerializeField] private string fallParameterName = "Fall";
    
    //Attack State
    [SerializeField] private string attackParameterName = "@Attack";
    [SerializeField] private string comboAttackParameterName = "ComboAttack";
    [SerializeField] private string skillParameterName = "Skill";
    
    //--------------------Hash------------------//
    
    //Move State
    public int MoveParameterHash { get; private set; }
    public int IdleParameterHash { get; private set; }
    public int DashParameterHash { get; private set; }
    public int RunParameterHash { get; private set; }
    public int WalkParameterHash { get; private set; }

    //Ground State
    public int GroundParameterHash { get; private set; }
    public int JumpParameterHash { get; private set; }
    public int FallParameterHash { get; private set; }
    
    //Attack State
    public int AttackParameterHash { get; private set; }
    public int ComboAttackParameterHash { get; private set; }
    public int SkillParameterHash { get; private set; }
    
    //--------------------초기화------------------//
    public void Initialize()
    {
        //Move State
        MoveParameterHash = Animator.StringToHash(moveParameterName);
        IdleParameterHash = Animator.StringToHash(idleParameterName);
        DashParameterHash = Animator.StringToHash(dashParameterName);
        RunParameterHash = Animator.StringToHash(runParameterName);
        WalkParameterHash = Animator.StringToHash(walkParameterName);
        
        //Ground State
        GroundParameterHash = Animator.StringToHash(groundParameterName);
        JumpParameterHash = Animator.StringToHash(jumpParameterName);
        FallParameterHash = Animator.StringToHash(fallParameterName);
        
        //Attack State
        AttackParameterHash = Animator.StringToHash(attackParameterName);
        ComboAttackParameterHash = Animator.StringToHash(comboAttackParameterName);
        SkillParameterHash = Animator.StringToHash(skillParameterName);
    }
}
