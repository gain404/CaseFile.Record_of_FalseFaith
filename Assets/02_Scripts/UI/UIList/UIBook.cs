using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIBook : MonoBehaviour
{
    [SerializeField] private float pageSpeed = 0.5f;
    [SerializeField] private List<Transform> pages;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backButton;

    [SerializeField]private int _index;//저장해놓기
    private bool _isRotating;
    private int _targetIndex;
    private bool _isFastRotating;

    private void Start()
    {
        InitState();
    }

    private void OnEnable()
    {
        //전 페이지 기억
    }

    private void InitState()
    {
        _index = -1;
        _isRotating = false;
        forwardButton.onClick.AddListener(RotateForward);
        backButton.onClick.AddListener(RotateBack);
        foreach (Transform page in pages)
        {
            page.transform.rotation = Quaternion.identity;
        }
        pages[0].SetAsFirstSibling();
        backButton.gameObject.SetActive(false);
    }

    public void RotateForward()
    {
        if (_isRotating || _isFastRotating) return;
        _index++;
        NextButtonAction();
        pages[_index].SetAsLastSibling();
        RotatePage(180f, pageSpeed,true);
    }

    private void NextButtonAction()
    {
        if (!backButton.gameObject.activeInHierarchy)
        {
            backButton.gameObject.SetActive(true);
        }

        if (_index == pages.Count - 1)
        {
            forwardButton.gameObject.SetActive(false);
        }
    }

    public void RotateBack()
    {
        if (_isRotating || _isFastRotating) return;
        BackButtonAction();
        pages[_index].SetAsLastSibling();
        RotatePage(0f, pageSpeed,false);
    }

    private void BackButtonAction()
    {
        if (!forwardButton.gameObject.activeInHierarchy)
        {
            forwardButton.gameObject.SetActive(true);
        }

        if (_index == 0)
        {
            backButton.gameObject.SetActive(false);
        }
    }
    
    //어떤 페이지의 왼쪽이 몇page인지 index 넣기
    //이건 addlistener보다 인스펙터 창에서 직접 할당해주는게 좋을듯
    public void ChoosePage(int index)
    {
        if (_isFastRotating || _isRotating) return;
        _targetIndex = index;
        FastRotate();
    }

    private void FastRotate()
    {
        if(_index == _targetIndex - 1)
        {
            _isFastRotating = false;
            return;
        }

        _isFastRotating = true;
        if(_index < _targetIndex -1)
        {
            _index++;
            NextButtonAction();
            pages[_index].SetAsLastSibling();
            RotatePage(180f, 0.2f,true, FastRotate);
        }
        else
        {
            BackButtonAction();
            pages[_index].SetAsLastSibling();
            RotatePage(0f, 0.2f,false, FastRotate);
        }
    }

    private void RotatePage(float angleY, float speed, bool isForward, System.Action onComplete = null)
    {
        _isRotating = true;
        pages[_index].DOLocalRotate(new Vector3(0, angleY, 0), speed)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                if (!isForward) _index--;
                _isRotating = false;
                onComplete?.Invoke(); // 콜백 함수 실행
            });
    }
}
