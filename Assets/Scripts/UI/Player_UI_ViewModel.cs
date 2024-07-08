using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player_UI_ViewModel : MonoBehaviour
{
    private float _hP;
    public float HP
    {
        get { return _hP; }
        set 
        {
            if (value == _hP) return;            
            _hP = value;
            OnPropertyChanged(nameof(HP));
        }
    }
    private float _stamina;
    public float Stamina
    {
        get { return _stamina; }
        set
        {
            if(_stamina == value) return;
            _stamina = value;
            OnPropertyChanged(nameof(Stamina));
        }
    }
    private float _lifeCount;
    public float LifeCount
    {
        get { return _lifeCount; }
        set
        {
            if(value == _lifeCount) return;
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