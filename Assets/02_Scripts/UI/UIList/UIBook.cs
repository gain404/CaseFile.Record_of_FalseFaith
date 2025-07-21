using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIBook : MonoBehaviour
{
    [SerializeField] private float pageSpeed = 0.5f;
    [SerializeField] private List<Transform> pages;
    [SerializeField] private GameObject forwardButton;
    [SerializeField] private GameObject backButton;

    private int _index;//저장해놓기
    private bool _isRotating;

    private void Awake()
    {
        _index = -1;
        backButton.SetActive(false);
    }

    public void RotateForward()
    {
        if (_isRotating) return;
        _index++;
        NextButtonAction();
        pages[_index].SetAsLastSibling();
        RotatePage(180f, true);
    }

    private void NextButtonAction()
    {
        if (!backButton.activeInHierarchy)
        {
            backButton.SetActive(true);
        }

        if (_index == pages.Count - 1)
        {
            forwardButton.SetActive(false);
        }
    }

    public void RotateBack()
    {
        if (_isRotating) return;
        BackButtonAction();
        pages[_index].SetAsLastSibling();
        RotatePage(0f, false);
    }

    private void BackButtonAction()
    {
        if (!forwardButton.activeInHierarchy)
        {
            forwardButton.SetActive(true);
        }

        if (_index == 0)
        {
            backButton.SetActive(false);
        }
    }

    private void RotatePage(float angleY, bool forward)
    {
        pages[_index].DOLocalRotate(new Vector3(0, angleY, 0), pageSpeed)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                if (!forward)
                    _index--;
            });
    }
}
