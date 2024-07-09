using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image _staminaBarLeft;
    public Image StaminaBarLeft { get { return _staminaBarLeft; } }
    [SerializeField] private Image _staminaBarRight;
    public Image StaminaBarRight { get {  return _staminaBarRight; } }

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
        StaminaBarLeft.fillAmount = fillAmount;
        _staminaBarRight.fillAmount = fillAmount;
    }
}
