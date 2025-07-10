using UnityEngine;
using UnityEngine.UI;

public class StaminaUI : MonoBehaviour
{
    /// <summary>
    /// UI에 표시될 스테미나 정보
    /// </summary>
    public Image staminaBar;

    public void UpdateStamina(float currentStamina, float maxStamina)
    {
        float fillAmount = currentStamina / maxStamina;
        staminaBar.fillAmount = fillAmount;
    }
}
