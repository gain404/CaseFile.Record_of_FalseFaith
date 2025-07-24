using UnityEngine;

[CreateAssetMenu(fileName = "InvestigationData", menuName = "Scriptable Objects/InvestigationData")]
public class InvestigationData : ScriptableObject
{
    public int chapter;
    public int indexNumber;
    public string investigationName;
    public string investigationDescription;
}
