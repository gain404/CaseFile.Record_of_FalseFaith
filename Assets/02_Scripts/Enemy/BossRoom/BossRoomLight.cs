using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BossRoomLight : MonoBehaviour
{
    public int BreakCount { get; private set; }
    
    [SerializeField] private Sprite crackBulb;
    [SerializeField] private Sprite breakBulb;
    
    private SpriteRenderer _spriteRenderer;
    private Light2D _light2D;

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _light2D = GetComponent<Light2D>();
        BreakCount = 2;
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
            _light2D.enabled = false;
        }
    }
}
