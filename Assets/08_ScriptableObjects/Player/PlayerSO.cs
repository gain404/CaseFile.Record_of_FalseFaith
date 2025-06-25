using UnityEngine;
using System;

[Serializable]
public class PlayerMoveData
{
    [field : SerializeField][field : Range(0f, 25f)] public float Speed { get; set; }
    
    
    [field : Header("IdleData")]
    
    [field : Header("WalkData")]
    [field : SerializeField][field : Range(0f, 2f)] public float WalkSpeedModifier { get; set; }
    
    [field : Header("RunData")]
    [field : SerializeField][field : Range(0f, 2f)] public float RunSpeedModifier { get; set; }
    
    [field : Header("DashData")]
    [field : SerializeField][field : Range(0f, 2f)] public float DashSpeedModifier { get; set; }
}

[Serializable]
public class PlayerGroundData
{
    [field : Header("JumpData")]
    [field : SerializeField][field : Range(0f, 25f)] public float JumpForce { get; set; }
}

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/Player")]
public class PlayerSO : ScriptableObject
{
    [field : SerializeField] public PlayerMoveData MoveData { get; private set; }
    [field : SerializeField] public PlayerGroundData GroundData { get; private set; }
}
