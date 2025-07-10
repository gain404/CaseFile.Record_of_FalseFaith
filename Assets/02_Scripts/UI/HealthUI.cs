using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    /// <summary>
    /// UI에 표시될 하트 하나하나에 대한 스크립트입니다
    /// </summary>

    public GameObject heartContainer;
    public GameObject heartPrefab;
    public Sprite fullHeart, halfHeart, emptyHeart;

    public void UpdateHearts(int currentHP, int maxHP)
    {
        //일단 다 없애줌
        foreach (Transform child in heartContainer.transform)
            Destroy(child.gameObject);

        //최대 체력 나누기 2 해서 표시할 하트 개수 정함
        int heartCount = Mathf.CeilToInt(maxHP / 2f);
        for (int i = 0; i < heartCount; i++)
        {
            GameObject heart = Instantiate(heartPrefab, heartContainer.transform);
            Image heartImage = heart.GetComponent<Image>();

            int heartIndex = i * 2;
            if (currentHP > heartIndex + 1)
                heartImage.sprite = fullHeart;
            else if (currentHP > heartIndex)
                heartImage.sprite = halfHeart;
            else
                heartImage.sprite = emptyHeart;
        }
    }
}
