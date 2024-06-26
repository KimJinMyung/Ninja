using ActorStateMachine;
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
public enum monsterType
{
    monster_A,
    monster_B,
    monster_C,
    Boss
}

public enum MonsterFileType
{
    Monster_Info,
    Monster_Attack
}

public class Monster : MonoBehaviour
{
    [Header("Monster Type")]
    [SerializeField] protected monsterType type;

    [Header("Monster Mesh")]
    [SerializeField] protected GameObject monsterMesh;

    protected Monster_Status_ViewModel _monsterState;
    public Monster_Status_ViewModel MonsterViewModel { get { return _monsterState; } }

    protected Monster_data monster_Info;

    protected StateMachine _monsterStateMachine;
    public StateMachine MonsterStateMachine {  get { return _monsterStateMachine; } }

    public int monsterId { get; protected set; }

    public NavMeshAgent Agent { get; protected set; }
    public Animator animator { get; protected set; }

    protected virtual void Awake()
    {
        Agent = GetComponentInChildren<NavMeshAgent>();

        _monsterStateMachine = gameObject.AddComponent<StateMachine>();

        _monsterStateMachine.AddState(State.Idle, new Monster_IdleState(this));
        _monsterStateMachine.AddState(State.Walk, new Monster_PatrolState(this));
        _monsterStateMachine.AddState(State.Run, new Monster_TraceState(this)); 
        _monsterStateMachine.AddState(State.Battle, new Monster_BattleState(this));
        _monsterStateMachine.AddState(State.Alert, new Monster_AlertState(this));
        _monsterStateMachine.AddState(State.Circling, new Monster_CirclingState(this));
        _monsterStateMachine.AddState(State.Attack, new Monster_AttackState(this));
        _monsterStateMachine.AddState(State.Hurt, new Monster_HurtState(this));
        _monsterStateMachine.AddState(State.Incapacitated, new Monster_SubduedState(this));
        _monsterStateMachine.AddState(State.Die, new Monster_DeadState(this));

        _monsterStateMachine.InitState(State.Idle);
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

        _monsterState.RequestStateChanged(monsterId, State.Idle);

        SetMonsterInfo();
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
    }

    public float CombatMovementTimer {  get; protected set; }

    protected virtual void SetMonsterInfo()
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        monster_Info = monster;
        _monsterState.RequestMonsterInfoChanged(monsterId, monster_Info);

        //SetAttackMethod(monster);
    }

    private void Update()
    {
        _monsterStateMachine.OnUpdate();
        Debug.Log(_monsterState.MonsterState);

        
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

                //_attackRange = attack.AttackRange;
                //AttackDelay = attack.AttackSpeed;

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
        switch(e.PropertyName)
        {
            case nameof(_monsterState.MonsterState):
                _monsterStateMachine.ChangeState(_monsterState.MonsterState);
                break;
        }
    }
}
