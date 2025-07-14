using UnityEngine;
public enum SfxName
{
    //오디오 이름 => 부족할시 추가
    //1줄 : 공통  2줄 : 플레이어 전용  3줄 : 몬스터 전용  4줄 : 그 외
    WalkSound, RunSound, DeathSound,
    SwordSound, SwordSkillSound ,GunSound, GunSkillSound, EatSound,
    BiteSound, MeleeAttackSound, AreaSkillSound, HowlingSound, Phase2Sound, 
    LightBreakSound, DoorOpenSound, DoorCloseSound
}

[System.Serializable]
public struct SfxType
{
    public SfxName audioName;
    public AudioClip Sfxclip;
}