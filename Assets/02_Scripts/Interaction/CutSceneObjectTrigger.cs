using UnityEngine;

public class CutSceneObjectTrigger : MonoBehaviour
{
    private CutSceneSignalTrigger _trigger;

    private void Awake()
    {
        _trigger = GetComponentInParent<CutSceneSignalTrigger>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((1 << collision.gameObject.layer & _trigger.playerLayerMask) != 0)
        {
            _trigger.gameObject.SetActive(false);
        }
    }
}
