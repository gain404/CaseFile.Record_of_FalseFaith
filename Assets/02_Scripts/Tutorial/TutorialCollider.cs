using UnityEngine;

public class TutorialCollider : MonoBehaviour
{
    [SerializeField] private TutorialType tutorialType;
    [SerializeField] private LayerMask layerMask;
    
    private UIManager _uiManager;
    private bool _isOpen;
    
    private void Start()
    {
        _uiManager = UIManager.Instance;
        _isOpen = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isOpen) return;
        if (((1 << other.gameObject.layer) & layerMask) == 0) return;
        _isOpen = true;
        _uiManager.UITutorial.OnTutorialPanel(tutorialType);
    }
}
