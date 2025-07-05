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
    //[SerializeField] private string fallParameterName = "Fall";
    
    //Attack State
    [SerializeField] private string actionParameterName = "@Action";
    [SerializeField] private string swordAttackParameterName = "SwordAttack";
    [SerializeField] private string gunAttackParameterName = "GunAttack";
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
    //public int FallParameterHash { get; private set; }
    
    //Attack State
    public int ActionParameterHash { get; private set; }
    public int SwordAttackParameterHash { get; private set; }
    public int GunAttackParameterHash { get; private set; }
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
        //FallParameterHash = Animator.StringToHash(fallParameterName);
        
        //Attack State
        ActionParameterHash = Animator.StringToHash(actionParameterName);
        SwordAttackParameterHash = Animator.StringToHash(swordAttackParameterName);
        GunAttackParameterHash = Animator.StringToHash(gunAttackParameterName);
        ComboAttackParameterHash = Animator.StringToHash(comboAttackParameterName);
        SkillParameterHash = Animator.StringToHash(skillParameterName);
    }
}
