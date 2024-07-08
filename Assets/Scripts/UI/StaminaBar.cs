using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image StaminaBarLeft;
    [SerializeField] private Image StaminaBarRight;
    public float maxStamina = 100f;
    [SerializeField] private float currentStamina;

    void Start()
    {
        currentStamina = maxStamina;
        UpdateHPBar();
    }

    public void SetMaxStamina(float maxStamina)
    {
        this.maxStamina = maxStamina;
        UpdateHPBar();
    }

    public void SetCurrentStamina(float amount)
    {
        currentStamina = amount;
        UpdateHPBar();
    }

    //public void TakeStamina(float amount)
    //{
    //    currentStamina = Mathf.Clamp(currentStamina - amount, 0, maxStamina);
    //    UpdateHPBar();
    //}

    //public void HealStamina(float amount)
    //{
    //    currentStamina = Mathf.Clamp(currentStamina + amount, 0, maxStamina);
    //    UpdateHPBar();
    //}

    private void UpdateHPBar()
    {
        float fillAmount = (float)(currentStamina / maxStamina);
        Debug.Log("33 : " + fillAmount);
        StaminaBarLeft.fillAmount = fillAmount;
        StaminaBarRight.fillAmount = fillAmount;
    }
}
