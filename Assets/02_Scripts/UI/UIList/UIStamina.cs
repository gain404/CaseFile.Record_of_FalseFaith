using UnityEngine;
using UnityEngine.UI;

public class UIStamina : MonoBehaviour
{
    /// <summary>
    /// UI에 표시될 스테미나 정보
    /// </summary>
    [SerializeField] private Image staminaBar;

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        float fillAmount = currentStamina / maxStamina;
        staminaBar.fillAmount = fillAmount;
    }
}
