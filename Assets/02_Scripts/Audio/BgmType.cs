using UnityEngine;
public enum BgmName
{
    BasicBgm,
    FightBgm,
    StartBgm,
    EndingBgm
}

[System.Serializable]
public struct BgmType
{
    public BgmName bgmName;
    public AudioClip bgmClip;
}