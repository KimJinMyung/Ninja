using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Monster_UI : MonoBehaviour
{
    [SerializeField] private Transform Monster_Life;
    [SerializeField] private Image HP_Bar;
    [SerializeField] private StaminaBar StaminaBar;

    Monster_UI_ViewModel vm;
    Monster owner;

    private void OnEnable()
    {
        owner = GetComponentInParent<Monster>();

        if(vm == null)
        {
            vm = new Monster_UI_ViewModel();
            vm.PropertyChanged += OnPropertyChanged;
            vm.RegisterMonsterHPChanged(true, owner.monsterId);
            vm.RegisterMonsterMaxHPChanged(true, owner.monsterId);
            vm.RegisterMonsterStaminaChanged(true, owner.monsterId);
            vm.RegisterMonsterMaxStaminaChanged(true, owner.monsterId);
            vm.RegisterMonsterLifeCountChanged(true, owner.monsterId);
        }
    }

    private void OnDisable()
    {
        if(vm != null)
        {
            vm.RegisterMonsterLifeCountChanged(false, owner.monsterId);
            vm.RegisterMonsterMaxStaminaChanged(false, owner.monsterId);
            vm.RegisterMonsterStaminaChanged(false , owner.monsterId);
            vm.RegisterMonsterMaxHPChanged(false , owner.monsterId);
            vm.RegisterMonsterHPChanged(false,owner.monsterId);
            vm.PropertyChanged -= OnPropertyChanged;
            vm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch(e.PropertyName)
        {
            case nameof(vm.HP):
                HP_Bar.fillAmount = (float)vm.HP / vm.MaxHP;
                break;
            case nameof(vm.MaxHP):
                HP_Bar.rectTransform.sizeDelta += new Vector2();
                break;
            case nameof(vm.Stamina):
                StaminaBar.SetCurrentStamina(vm.Stamina);
                break;
            case nameof(vm.MaxStamina):
                StaminaBar.SetMaxStamina(vm.MaxStamina);
                break;
            case nameof(vm.LifeCount):
                int index = 1;
                foreach (Transform child in Monster_Life)
                {
                    if (index > vm.LifeCount)
                    {
                        child.gameObject.SetActive(false);
                        continue;
                    }

                    child.gameObject.SetActive(true);
                    index++;
                }
                break;
        }
    }
}
