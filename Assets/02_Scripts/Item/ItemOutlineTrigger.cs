using UnityEngine;

public class ItemOutlineTrigger : MonoBehaviour
{
    private Material _material;

    void Start()
    {
        SpriteRenderer sr = GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            _material = sr.material;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _material != null)
        {
            _material.SetFloat("_ShowOutline", 1);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && _material != null)
        {
            _material.SetFloat("_ShowOutline", 0);
        }
    }
}
