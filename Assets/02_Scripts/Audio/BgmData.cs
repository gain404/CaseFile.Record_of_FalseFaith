using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BgmData", menuName = "Scriptable Objects/BgmData")]
public class BgmData : ScriptableObject
{
    public List<BgmType> clips;
}
