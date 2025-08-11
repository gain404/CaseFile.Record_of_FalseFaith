using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossRoomLight : MonoBehaviour
{
    public int BreakCount;
    
    [SerializeField] private Sprite crackBulb;
    [SerializeField] private Sprite breakBulb;
    [SerializeField] private Light2D light2D;
    
    private SpriteRenderer _spriteRenderer;
    
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void BreakLight()
    {
        BreakCount--;
        if (BreakCount == 1)
        {
            _spriteRenderer.sprite = crackBulb;
        }
        else if (BreakCount == 0)
        {
            _spriteRenderer.sprite = breakBulb;
            light2D.enabled = false;
        }
    }
}
