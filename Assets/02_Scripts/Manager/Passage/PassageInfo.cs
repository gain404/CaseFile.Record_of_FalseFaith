using UnityEngine;

[CreateAssetMenu(fileName = "PassageInfo", menuName = "Scriptable Objects/PassageInfo")]
public class PassageInfo : ScriptableObject
{
    public Vector3 targetPosition;
    public string targetPositionName;
}
