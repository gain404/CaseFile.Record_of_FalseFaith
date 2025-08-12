using System.Collections.Generic;
using UnityEngine;

public class TutorialColliderController : MonoBehaviour
{
    [SerializeField] private List<TutorialCollider> tutorialCollider;
    
    private UIManager _uiManager;
    private void Start()
    {
        _uiManager = UIManager.Instance;
        // foreach (TutorialCollider tutorial in tutorialCollider)
        // {
        //     tutorial.gameObject.SetActive(false);
        // }
    }

    private void OnCollider()
    {
        foreach (TutorialCollider tutorial in tutorialCollider)
        {
            tutorial.gameObject.SetActive(true);
        }
    }
}
