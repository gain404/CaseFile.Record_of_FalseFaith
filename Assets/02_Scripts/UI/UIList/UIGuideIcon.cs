using System;
using System.Collections.Generic;
using UnityEngine;

public enum GuideIconType
{
    OpenDoor,
    CloseDoor,
    Investigation,
    Go,
    Stair,
    GoDownStreet,
    GoUpStreet,
}

public class UIGuideIcon : MonoBehaviour
{
    [SerializeField] private Sprite[] icons;
    
    private Dictionary<GuideIconType,Sprite> _guideIcon;
    private SpriteRenderer _spriteRenderer;

    private void Awake()
    {
        _guideIcon = new Dictionary<GuideIconType,Sprite>();

        foreach (var icon in icons)
        {
            if (Enum.TryParse(icon.name, out GuideIconType iconType))
            {
                _guideIcon[iconType] = icon;
            }
            else
            {
                Debug.LogWarning($"Icon {icon.name} not found");
            }
        }
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if(_spriteRenderer == null)
            Debug.LogWarning("SpriteRenderer not found");
    }

    private void Start()
    {
        _spriteRenderer.enabled = false;
        _spriteRenderer.sortingOrder = 25;
    }
    
    public void OnGuideIcon(GuideIconType guideIconType, Vector3 iconTransform)
    {
        Vector3 guidePosition = new Vector3(0, -1, 0);
        
        _spriteRenderer.sprite = _guideIcon[guideIconType];
        _spriteRenderer.transform.position = iconTransform + guidePosition;
        _spriteRenderer.enabled = true;
    }

    public void OffGuideIcon()
    {
        _spriteRenderer.enabled = false;
    }
    
}
