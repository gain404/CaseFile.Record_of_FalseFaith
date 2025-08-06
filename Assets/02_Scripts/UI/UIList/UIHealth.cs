using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIHealth : MonoBehaviour
{
    [SerializeField] private GameObject heartContainer;
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite fullHeart, halfHeart, emptyHeart;

    private List<Image> _heartList;
    private PlayerStat _playerStat;

    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        _playerStat = player.GetComponent<PlayerStat>();
        InitHeart();
    }

    private void InitHeart()
    {
        _heartList = new List<Image>();
        foreach (Transform child in heartContainer.transform)
        {
            Destroy(child.gameObject);
        }
        int heartCount = Mathf.CeilToInt((int)_playerStat.MaxHeart / 2f);
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer.transform);
            Image heartImage = heart.GetComponent<Image>();
            _heartList.Add(heartImage);
        }
    }

    public void UpdateHeart()
    {
        if (_heartList == null || _playerStat == null)
        {
            Debug.LogWarning("UpdateHeart가 너무 일찍 호출되었습니다. heartList가 null입니다.");
            return;
        }

        if (_heartList.Count * 2 < (int)_playerStat.MaxHeart)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer.transform);
            Image heartImage = heart.GetComponent<Image>();
            _heartList.Add(heartImage);
        }

        int currentHp = (int)_playerStat.CurrentHeart;
        for (int i = 0; i < _heartList.Count; i++)
        {
            int hpIndex = i * 2;
            if (currentHp > hpIndex + 1)
                _heartList[i].sprite = fullHeart;
            else if (currentHp > hpIndex)
                _heartList[i].sprite = halfHeart;
            else
                _heartList[i].sprite = emptyHeart;
        }
    }
}
