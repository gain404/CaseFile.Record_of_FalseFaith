using UnityEngine;
using System;
using System.Collections.Generic;

[Serializable]
public class PlayerMoveData
{
    [field : SerializeField][field : Range(0f, 25f)] public float Speed { get; set; }
    
    [field : Header("WalkData")]
    [field : SerializeField][field : Range(0f, 5f)] public float WalkSpeedModifier { get; set; }
    
    [field : Header("RunData")]
    [field : SerializeField][field : Range(0f, 5f)] public float RunSpeedModifier { get; set; }
    
    [field : Header("DashData")]
    [field : SerializeField][field : Range(0f, 20f)] public float DashSpeedModifier { get; set; }
}

[Serializable]
public class PlayerGroundData
{
    [field : Header("JumpData")]
    [field : SerializeField][field : Range(0f, 20f)] public float JumpForce { get; set; }
}

[System.Serializable]
public class BaseStat
{
    public StatType Type;
    public float Value;
}


// 클래스 이름을 PlayerData에서 CharacterData로 변경합니다.
[CreateAssetMenu(fileName = "CharacterData", menuName = "ScriptableObjects/CharacterData")]
public class CharacterData : ScriptableObject
{
    [Header("State Machine Data")]
    [field: SerializeField] public PlayerMoveData MoveData { get; private set; }
    [field: SerializeField] public PlayerGroundData GroundData { get; private set; }

    [Header("Flexible Stat System")]
    // CharacterStatSO의 역할을 이 리스트가 대신합니다.
    public List<BaseStat> Stats = new List<BaseStat>();
}
