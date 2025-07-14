using UnityEngine;
using Unity.Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public CinemachineCamera playerFollowCamera;
    public CinemachineCamera dialogueCamera;

    public void SwitchToDialogueCamera()
    {
        playerFollowCamera.Priority = 10;
        dialogueCamera.Priority = 20;
    }

    public void SwitchToPlayerCamera()
    {
        playerFollowCamera.Priority = 20;
        dialogueCamera.Priority = 0;
    }
}