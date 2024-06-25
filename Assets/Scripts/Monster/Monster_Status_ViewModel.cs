
using System.ComponentModel;
using UnityEngine;

public class Monster_Status_ViewModel
{
    private State _monsterState;
    public State MonsterState
    {
        get { return _monsterState; }
        set
        {
            if (_monsterState == value) return;
            _monsterState = value;
            OnPropertyChanged(nameof(MonsterState));
        }
    }

    private Monster_data _monster_info;
    public Monster_data MonsterInfo
    {
        get => _monster_info;
        set
        {
            if (_monster_info == value) return;

            _monster_info = value;
            OnPropertyChanged(nameof(MonsterInfo));
        }
    }

    private Transform _traceTarget;
    public Transform TraceTarget
    {
        get => _traceTarget;
        set
        {
            if (_traceTarget == value) return;

            _traceTarget = value;
            OnPropertyChanged(nameof(TraceTarget));
        }
    }

    #region propertyEvent

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
