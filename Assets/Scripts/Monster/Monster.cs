using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
public enum monsterType
{
    monster_A,
    monster_B,
    Boss
}

public class Monster : MonoBehaviour
{
    protected Monster_Status_ViewModel _monsterState;
    public Monster_Status_ViewModel MonsterViewModel { get { return _monsterState; } }

    protected Monster_data monster_Info;
    public NavMeshAgent Agent { get; protected set; }

    public int monsterId { get; protected set; }

    protected virtual void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
    }

    protected virtual void OnEnable()
    {
        monsterId = this.gameObject.GetInstanceID();

        if(_monsterState == null)
        {
            _monsterState = new Monster_Status_ViewModel();
            _monsterState.PropertyChanged += OnPropertyChanged;
            _monsterState.RegisterStateChanged(monsterId, true);
            _monsterState.RegisterMonsterInfoChanged(monsterId, true);
            _monsterState.RegisterTraceTargetChanged(monsterId, true);
        }
    }

    protected virtual void OnDisable()
    {
        if(_monsterState != null)
        {
            _monsterState.RegisterTraceTargetChanged(monsterId, false);
            _monsterState.RegisterMonsterInfoChanged(monsterId, false);
            _monsterState.RegisterStateChanged(monsterId, false);
            _monsterState.PropertyChanged -= OnPropertyChanged;
            _monsterState = null;
        }

        SetMonsterInfo();
    }

    public float CombatMovementTimer {  get; protected set; }

    protected monsterType type;

    protected virtual void SetMonsterInfo()
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        monster_Info = monster;
        _monsterState.RequestMonsterInfoChanged(monsterId, monster_Info);

        SetAttackMethod(monster);
    }

    protected float _attackRange = 0;
    public float AttackRange { get { return _attackRange; } }
    public float AttackDelay { get; protected set; }

    //두개 이상의 공격 방식을 가지고 있다면
    //플레이어와의 거리가 가까우면 근접으로 멀면 원거리로 변경
    protected virtual void SetAttackMethod(Monster_data monster)
    {
        var attakList = monster.AttackMethodName;
        if (attakList.Count > 0)
        {
            foreach (string attackName in attakList)
            {
                var attack = DataManager.Instance.GetAttackMethodName(attackName);
                string attackScriptName = attack.AttackScriptName;

                _attackRange = attack.AttackRange;
                AttackDelay = attack.AttackSpeed;

                Type atk = Type.GetType(attackScriptName);
                gameObject.AddComponent(atk);
            }
        }
    }

    public bool IsCurrentState(State state)
    {
        return _monsterState.MonsterState == state;
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
    }
}
