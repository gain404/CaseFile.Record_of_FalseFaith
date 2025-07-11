using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]

public class AudioData : ScriptableObject
{
    public List<AudioType> clips;
}
