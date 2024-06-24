using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Monster_B : Monster
{
    protected override void Awake()
    {
        base.Awake();
        type = monsterType.monster_B;
        _viewAngle = 90f;
        animator = GetComponent<Animator>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    #region PropertyChangedMethod
    protected override void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(sender, e);
    }
    #endregion
}
