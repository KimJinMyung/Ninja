using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;
using static UnityEngine.UI.GridLayoutGroup;

public enum monsterType
{
    monster_A,
    Boss
}

public class Monster : MonoBehaviour
{    
    //임시
    //[SerializeField] protected float _hp;
    //public float HP { get { return _hp; } }


    [SerializeField] protected float _detectionRadius;
    [SerializeField] protected float _attackRange;
    [SerializeField] protected float _attackDelay;
    public float AttackDelay { get { return _attackDelay; } }

    [Header("DetectZone")]
    [SerializeField] private GameObject _detectZone;
    [SerializeField] protected float _viewAngle;
    public float ViewAngle {  get { return _viewAngle; } }

    #region InstanceID
    protected int _monsterId;
    public int monsterId
    {
        get { return _monsterId; }
    }
    #endregion
    protected monsterType type;
    //스폰할 몬스터 종류

    protected StateMachine _stateMachine;

    protected Monster_Status_ViewModel _monsterState;
    public Monster_Status_ViewModel MonsterViewModel { get { return _monsterState; } }

    private Monster_data monster_Info;
    public Monster_data Monster_Info { get { return monster_Info; } set { monster_Info = value; } }

    [SerializeField] protected bool isPatrolMonster;
    [SerializeField] protected bool isRandomPatrolMonster;
    public bool IsPatrolMonster { get { return isPatrolMonster; } }
    public bool IsRandomPatrolMonster { get { return isRandomPatrolMonster;} }

    protected NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }
    protected Animator animator;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

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
        }
        else 
        {
            _monsterState.RequestStateChanged(monsterId, State.Idle);
        }
    }

    protected virtual void Start()
    {
        _monsterState.RegisterStateChanged(_monsterId, true);
        _monsterState.RegisterMonsterInfoChanged(_monsterId, true);
        _monsterState.RegisterTraceTargetChanged(_monsterId, true);

        _monsterState.RequestStateChanged(monsterId, State.Idle);

        SetMonsterInfo();

        GameObject detectZone = Instantiate(_detectZone, transform.position, transform.rotation);
        detectZone.transform.parent = transform;

        //_stateMachine.ChangeState(State.Walk);
       // _monsterState.RequestStateChanged(_monsterId, State.Walk);
    }

    protected virtual void SetMonsterInfo()
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        monster_Info = monster;        

        SetAttackMethod(monster);
    }

    protected virtual void SetAttackMethod(Monster_data monster)
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
            _monsterState.RegisterMonsterInfoChanged(_monsterId, false);
            _monsterState.RegisterStateChanged(_monsterId, false);
            _monsterState.RegisterTraceTargetChanged(_monsterId, false);
            _monsterState.PropertyChanged -= OnPropertyChanged;
            _monsterState = null;
        }
    }
    protected virtual void Update()
    {
        MonsterAI();
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_monsterState.MonsterState):
                _stateMachine.ChangeState(_monsterState.MonsterState);
                NewStateEnter(_monsterState.MonsterState);
                break;
        }
    }

    protected virtual void NewStateEnter(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                _time = 0f;
                _PatrolDelay = UnityEngine.Random.Range(0.2f, 3f);
                agent.speed = MonsterViewModel.MonsterInfo.WalkSpeed;
                break;
            case State.Walk:
                ////if (owner.IsPatrolMonster)
                ////{
                ////    if (owner.IsRandomPatrolMonster)
                ////    {
                ////        RandomPoint();
                ////    }
                ////    else
                ////    {
                ////        //패트롤 지점 지정
                ////    }
                ////}
                StartPos = transform.position;
                patrolEndPos = StartPos;
                RandomPoint();
                break;
            case State.Trace:
                agent.speed = MonsterViewModel.MonsterInfo.RunSpeed;
                break;
            case State.Alert:
                _timer = 0f;
                break;
        }
    }

    #region PatrolState
    private float _time;
    private float _PatrolDelay;
    public float PatrolDelay { get => _PatrolDelay; set { _PatrolDelay = value; } }

    protected Vector3 patrolEndPos;
    protected Vector3 StartPos;

    [Header("Patrol 가능 길이")]
    [SerializeField] protected float _distance = 15f;
    #endregion
    #region AlertState
    protected float _timer;
    protected float AlertStateEndTime = 5f;
    #endregion

    protected virtual void RandomPoint()
    {
        while (patrolEndPos == transform.position)
        {
            Vector3 randomPoint = transform.position + UnityEngine.Random.insideUnitSphere * _distance;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, (1 << NavMesh.GetAreaFromName("Walkable"))))
            {
                patrolEndPos = hit.position;
                break;
            }
        }
    }

    protected void MonsterAI()
    {
        switch (MonsterViewModel.MonsterState)
        {
            case State.Idle:
                if(MonsterViewModel.MonsterInfo.HP <= 0)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Die);
                    return;
                }

                if (MonsterViewModel.TraceTarget != null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }
                else
                {
                    if (!IsPatrolMonster) return;

                    _time = Mathf.Clamp(_time + Time.deltaTime, 0f, _PatrolDelay);
                    if (_time >= _PatrolDelay)
                    {
                        MonsterViewModel.RequestStateChanged(monsterId, State.Walk);
                    }
                }
                break;
            case State.Walk:
                if (MonsterViewModel.TraceTarget != null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }
                else
                {
                    if (patrolEndPos != StartPos && Vector3.Distance(transform.position, patrolEndPos) <= 0.2f)
                    {
                        MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
                    }
                }
                break;
            case State.Trace:                
                if (MonsterViewModel.TraceTarget == null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
                    return;
                }

                agent.SetDestination(MonsterViewModel.TraceTarget.position);
                break;
            case State.Alert:
                if (MonsterViewModel.TraceTarget != null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }
                else
                {
                    _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, AlertStateEndTime);
                    if (_timer >= AlertStateEndTime)
                    {
                        MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
                    }
                }
                break;
        }
    }
}
