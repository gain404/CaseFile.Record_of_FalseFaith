using UnityEngine;

public class ItemOutlineTrigger : MonoBehaviour
{
    [SerializeField] private GuideIconType guideIconType;
    private Material _material;
    private Vector3 _iconPosition;

    void Start()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            _material = sr.material;
        }

        _iconPosition = transform.position + Vector3.up * 3f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _material != null)
        {
            _material.SetFloat("_ShowOutline", 1);
            UIManager.Instance.UIGuideIcon.OnGuideIcon(guideIconType, _iconPosition);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _material != null)
        {
            _material.SetFloat("_ShowOutline", 0);
            UIManager.Instance.UIGuideIcon.OffGuideIcon();
        }
    }
}
