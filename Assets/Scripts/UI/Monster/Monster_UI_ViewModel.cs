using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Monster_UI_ViewModel : MonoBehaviour
{
    private float _hp;
    public float HP
    {
        get { return _hp; }
        set
        {
            if (value == _hp) return;
            _hp = value;
            OnPropertyChanged(nameof(HP));
        }
    }
    private float _maxHP;
    public float MaxHP
    {
        get { return _maxHP; }
        set
        {
            if (value == _maxHP) return;
            _maxHP = value;
            OnPropertyChanged(nameof(MaxHP));
        }
    }
    private float _stamina;
    public float Stamina
    {
        get { return _stamina; }
        set
        {
            if (_stamina == value) return;
            _stamina = value;
            OnPropertyChanged(nameof(Stamina));
        }
    }
    private float _maxStamina;
    public float MaxStamina
    {
        get { return _maxStamina; }
        set
        {
            if (_maxStamina == value) return;
            _maxStamina = value;
            OnPropertyChanged(nameof(MaxStamina));
        }
    }
    private float _lifeCount;
    public float LifeCount
    {
        get { return _lifeCount; }
        set
        {
            if (value == _lifeCount) return;
            _lifeCount = value;
            OnPropertyChanged(nameof(LifeCount));
        }
    }

    #region PropChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
