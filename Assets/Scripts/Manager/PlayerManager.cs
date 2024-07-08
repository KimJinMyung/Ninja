using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    Action<float> HPChangedEvnetHandler;
    Action<float> StaminaUpdateEvnetHandler;
    Action<float> LifeCountUpdateEventHandler;

    private void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(gameObject);
    }

    Player_data player_data;

    public void BindHPChanged(Action<float> HPChanged, bool isBind)
    {
        if(isBind) HPChangedEvnetHandler += HPChanged;
        else HPChangedEvnetHandler -= HPChanged;
    }

    public void BindStaminaChanged(Action<float> StaminaChanged, bool isBind)
    {
        if (isBind) StaminaUpdateEvnetHandler += StaminaChanged;
        else StaminaUpdateEvnetHandler -= StaminaChanged;
    }
    public void BindLifeCountChanged(Action<float> LifeCountChanged, bool isBind)
    {
        if (isBind) LifeCountUpdateEventHandler += LifeCountChanged;
        else LifeCountUpdateEventHandler -= LifeCountChanged;
    }

    public void SetPlayer_data(Player_data player_data)
    {
        this.player_data = player_data;

        HPChangedEvnetHandler?.Invoke(player_data.HP);
        StaminaUpdateEvnetHandler?.Invoke(player_data.Stamina);
        LifeCountUpdateEventHandler?.Invoke(player_data.Life);
    }

    public Player_data GetPlayer_data()
    {
        return this.player_data;
    }

    private void Update()
    {
        Debug.Log("3. : " + player_data.HP);
    }
}
