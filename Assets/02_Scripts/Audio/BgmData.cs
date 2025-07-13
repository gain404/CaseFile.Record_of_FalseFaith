using UnityEngine;

[CreateAssetMenu(fileName = "BgmData", menuName = "Scriptable Objects/BgmData")]
public class BgmData : ScriptableObject
{
    public AudioClip basicBgm;
    public AudioClip fightBgm;
}
