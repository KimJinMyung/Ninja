
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Monster_Status_ViewModel
{
    private MonsterType _monsterType;
    public MonsterType MonsterType
    {
        get { return _monsterType; }
        set
        {
            if (_monsterType == value) return;
            _monsterType = value;
            OnPropertyChanged(nameof(MonsterType));
        }
    }

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

    private Monster_Attack _currentAttackMethod;
    public Monster_Attack CurrentAttackMethod
    {
        get { return _currentAttackMethod; }
        set
        {
            if(value == _currentAttackMethod) return;
            _currentAttackMethod = value;
            OnPropertyChanged(nameof(CurrentAttackMethod));
        }
    }

    #region propertyEvent

    public event PropertyChangedEventHandler PropertyChanged;

    public virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
