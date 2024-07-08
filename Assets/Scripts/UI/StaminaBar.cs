using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [SerializeField] private Image StaminaBarLeft;
    [SerializeField] private Image StaminaBarRight;
    public float maxHP = 100f;
    [SerializeField] private float currentHP;

    void Start()
    {
        currentHP = maxHP;
        UpdateHPBar();
    }

    private void Update()
    {
        UpdateHPBar();
    }

    public void TakeDamage(float amount)
    {
        currentHP = Mathf.Clamp(currentHP - amount, 0, maxHP);
        UpdateHPBar();
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, maxHP);
        UpdateHPBar();
    }

    private void UpdateHPBar()
    {
        float fillAmount = currentHP / maxHP;
        StaminaBarLeft.fillAmount = fillAmount;
        StaminaBarRight.fillAmount = fillAmount;
    }
}
