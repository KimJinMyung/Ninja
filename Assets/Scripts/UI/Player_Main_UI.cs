using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class Player_Main_UI : MonoBehaviour
{
    [SerializeField] private Image HP_Bar;
    [SerializeField] private StaminaBar StaminaBar;
    [SerializeField] private Transform player_Life;

    Player_UI_ViewModel ui_Viewmodel;

    private void OnEnable()
    {
        if(ui_Viewmodel == null)
        {
            ui_Viewmodel = new Player_UI_ViewModel();
            ui_Viewmodel.PropertyChanged += OnPropertyChanged;
            ui_Viewmodel.RegisterPlayerHPChanged(true);
            ui_Viewmodel.RegisterPlayerStaminaChanged(true);
            ui_Viewmodel.RegisterPlayerLifeCountChanged(true);
        }        
    }

    private void OnDisable()
    {
        if (ui_Viewmodel != null)
        {
            ui_Viewmodel.RegisterPlayerHPChanged(false);
            ui_Viewmodel.RegisterPlayerStaminaChanged(false);
            ui_Viewmodel.RegisterPlayerLifeCountChanged(false);
            ui_Viewmodel.PropertyChanged -= OnPropertyChanged;
            ui_Viewmodel = null;
        }
    }

    private void Update()
    {
        Debug.Log("1. : " + ui_Viewmodel.HP);
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
        switch(e.PropertyName)
        {
            case nameof(ui_Viewmodel.HP):
                
                break;
        }
    }
}
