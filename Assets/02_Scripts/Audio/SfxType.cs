using UnityEngine;
public enum SfxName
{
    //오디오 이름 => 부족할시 추가
    //1줄 : 공통  2줄 : 플레이어 전용  3줄 : 몬스터 전용  4줄 : 그 외
    Walk, Run, Death,
    Sword, SwordSkill ,Gun, GunSkill, Eat,
    BiteSuccess, BiteFailed, MeleeAttackSuccess,MeleeAttackFailed, AreaSkill, Howling, Phase2, 
    LightBreak, DoorOpen, DoorClose
}

[System.Serializable]
public struct SfxType
{
    public SfxName audioName;
    public AudioClip Sfxclip;
}