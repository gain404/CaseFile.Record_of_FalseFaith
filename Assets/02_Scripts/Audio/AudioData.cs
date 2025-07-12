using System.Collections.Generic;
using UnityEngine;

public enum AudioName
{
    //오디오 이름 => 부족할시 추가
    //1줄 : 공통  2줄 : 플레이어 전용  3줄 : 몬스터 전용  4줄 : 그 외
    WalkSound, RunSound, DeathSound,
    SwordSound, SwordSkillSound ,GunSound, GunSkillSound, EatSound,
    BiteSound, MeleeAttackSound, AreaSkillSound, HowlingSound, Phase2Sound, 
    LightBreakSound, DoorOpenSound, DoorCloseSound
}

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]

public class AudioData : ScriptableObject
{
    public List<AudioType> clips;
}
