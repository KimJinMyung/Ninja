using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    Action<float> HPChangedEvnetHandler;

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

    public void SetPlayer_data(Player_data player_data)
    {
        this.player_data = player_data;

        HPChangedEvnetHandler?.Invoke(player_data.HP);
    }

    public Player_data GetPlayer_data()
    {
        return this.player_data;
    }

    private void Update()
    {
        Debug.Log(player_data.HP);
    }
}
