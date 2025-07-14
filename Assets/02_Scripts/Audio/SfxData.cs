using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SfxData", menuName = "Scriptable Objects/SfxData")]

public class SfxData : ScriptableObject
{
    public List<SfxType> clips;
}
