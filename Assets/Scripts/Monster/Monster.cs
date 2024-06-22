using System;
using System.ComponentModel;
using UnityEngine;

public enum monsterType
{
    monster_A,
    Boss
}

public class Monster : MonoBehaviour
{    
    [SerializeField] protected float _hp;
    public float HP { get { return _hp; } }
    [SerializeField] protected float _detectionRadius;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackDelay;

    [Header("DetectZone")]
    [SerializeField] private GameObject _detectZone;
    protected float lastAttackTime;

    protected int _monsterId;
    public int monsterId
    {
        get { return _monsterId; }
    }

    protected monsterType type;
    //스폰할 몬스터 종류

    protected StateMachine _stateMachine;

    protected Monster_Status_ViewModel _monsterState;
    public Monster_Status_ViewModel MonsterViewModel { get { return _monsterState; } }

    protected virtual void Awake()
    {
        _stateMachine = gameObject.AddComponent<StateMachine>();

        _stateMachine.AddState(State.Idle, new Monster_IdleState(this));
        _stateMachine.AddState(State.Walk, new Monster_PatrolState(this));
        _stateMachine.AddState(State.Trace, new Monster_TraceState(this));
        _stateMachine.AddState(State.Alert, new Monster_AlertState(this));
        _stateMachine.AddState(State.Incapacitated, new Monster_SubduedState(this));
        _stateMachine.AddState(State.Battle, new Monster_BattleState(this));
        _stateMachine.AddState(State.Attack, new Monster_AttackState(this));
        _stateMachine.AddState(State.Die, new Monster_DeadState(this));

        _stateMachine.InitState(State.Idle);
    }

    protected virtual void OnEnable()
    {
        _monsterId = GetInstanceID();

        if (_monsterState == null)
        {
            _monsterState = new Monster_Status_ViewModel();
            _monsterState.PropertyChanged += OnPropertyChanged;
            _monsterState.RegisterStateChanged(_monsterId, true);
            _monsterState.RegisterHPChanged(_monsterId, true);            
        }

        lastAttackTime -= _attackDelay;
    }

    protected virtual void Start()
    {
        SetMonsterInfo();

        gameObject.AddComponent<Monster_Ai>().DetectZone = _detectZone;
    }

    protected void SetMonsterInfo()
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        _hp = monster.HP;

        SetAttackMethod(monster);
    }

    protected void SetAttackMethod(Monster_data monster)
    {
        var attakList = monster.AttackMethodName;
        if (attakList.Count > 0)
        {
            foreach(var attackName in attakList)
            {
                var attack = DataManager.Instance.GetAttackMethodName(attackName);
                string attackScriptName = attack.AttackScriptName;
                Type atk = Type.GetType(attackScriptName);
                gameObject.AddComponent(atk);
            }
        }
    }

    protected virtual void OnDisable()
    {
        if (_monsterState != null)
        {
            _monsterState.RegisterHPChanged(_monsterId, false);
            _monsterState.RegisterStateChanged(_monsterId, false);
            _monsterState.PropertyChanged -= OnPropertyChanged;
            _monsterState = null;
        }
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_monsterState.MonsterState):
                _stateMachine.ChangeState(_monsterState.MonsterState);
                break;
        }
    }
}
