using UnityEngine;

[CreateAssetMenu(fileName = "PatrolPoint", menuName = "Scriptable Objects/PatrolPoint")]
public class PatrolPoint : ScriptableObject
{
    public int floor;
    public float leftEnd;
    public float rightEnd;
    public float zPosition;
}
