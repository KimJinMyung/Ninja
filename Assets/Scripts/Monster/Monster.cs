using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;
using Cinemachine;
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

    protected Monster_data monster_Info;
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
        animator = GetComponent<Animator>();

        _stateMachine = gameObject.AddComponent<StateMachine>();

        _stateMachine.AddState(State.Idle, new Monster_IdleState(this));
        _stateMachine.AddState(State.Walk, new Monster_PatrolState(this));
        _stateMachine.AddState(State.Trace, new Monster_TraceState(this));
        _stateMachine.AddState(State.Alert, new Monster_AlertState(this));
        _stateMachine.AddState(State.Incapacitated, new Monster_SubduedState(this));
        _stateMachine.AddState(State.Battle, new Monster_BattleState(this));
        _stateMachine.AddState(State.Circling, new Monster_CirclingState(this));
        _stateMachine.AddState(State.Attack, new Monster_AttackState(this));
        _stateMachine.AddState(State.Hurt, new Monster_HurtState(this));
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

    protected void Init_IdleState()
    {
        _time = 0f;
        _PatrolDelay = UnityEngine.Random.Range(0.2f, 3f);
        agent.speed = monster_Info.WalkSpeed;
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
        Init_IdleState();
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

    protected virtual void FixedUpdate()
    {
        Debug.Log(_monsterState.MonsterState);        

        MonsterAI();        

        animator.SetFloat("MoveSpeed", (float)agent.speed / _agentSpeed);
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_monsterState.MonsterState):
                _stateMachine.ChangeState(_monsterState.MonsterState);
                NewStateEnter(_monsterState.MonsterState);
                break;
            case nameof(_monsterState.TraceTarget):
                if (_monsterState.TraceTarget == null) animator.SetBool("ComBatMode", true);
                else animator.SetBool("ComBatMode", false);
                break;
        }
    }

    protected virtual void NewStateEnter(State newState)
    {
        switch (newState)
        {
            case State.Idle:
                Init_IdleState();
                agent.angularSpeed = 1000;
                Debug.Log("시작");
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
                _agentSpeed = monster_Info.WalkSpeed;
                StartPos = transform.position;
                patrolEndPos = StartPos;
                RandomPoint();
                agent.destination = patrolEndPos;
                break;
            case State.Trace:
                _agentSpeed = monster_Info.RunSpeed;
                agent.angularSpeed = 3000;
                _distanceToTarget = 0;                
                traceTargetPos = MonsterViewModel.TraceTarget.position;
                agent.SetDestination(traceTargetPos);
                break;
            case State.Alert:
                _time = 0f;
                break;
            case State.Incapacitated:
                _time = 0f;
                break;
            case State.Battle:
                _time = 0f;
                _circleDelay = UnityEngine.Random.Range(2f, 5f);
                agent.destination = default;
                animator.SetBool("Circling", false);
                agent.speed = 0f;
                break;
            case State.Circling:
                _time = 0f;
                _circleTimeRange = UnityEngine.Random.Range(3f, 6f);
                _circlingDir = UnityEngine.Random.Range(0, 2) == 0 ? 1 : -1;   
                agent.ResetPath();
                _agentSpeed = monster_Info.WalkSpeed;
                animator.SetBool("Circling", true);
                break;

        }
    }

    protected float _agentSpeed;
    #region PatrolState
    private float _time;
    private float _PatrolDelay;
    public float PatrolDelay { get => _PatrolDelay; set { _PatrolDelay = value; } }

    protected Vector3 patrolEndPos;
    protected Vector3 StartPos;

    [Header("Patrol 가능 길이")]
    [SerializeField] protected float _distance = 15f;
    #endregion
    #region Trace
    [Header("Trace 여유 범위")]
    [SerializeField] protected float _distanceToStand = 4f;

    protected float _distanceToTarget;
    protected Vector3 traceTargetPos;
    #endregion
    #region AlertState
    protected float AlertStateEndTime = 5f;
    #endregion
    #region SubduedState
    [Header("기절 시간")]
    [SerializeField] protected float SubduedEndTime;
    protected bool isSubdued;
    public bool IsSubdued {  get { return isSubdued; } set { isSubdued = value; } }
    #endregion
    #region BattleState
    protected float _circleDelay;
    //임시
    protected float _attackDelay;
    protected float _attackRange;
    #endregion
    #region Circling
    protected float _circleTimeRange;
    protected float _circlingSpeed;
    protected int _circlingDir = 1;
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
                if(monster_Info.HP <= 0)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Die);
                    return;
                }

                agent.speed = Mathf.Lerp(agent.speed, 0f, 10f * Time.fixedDeltaTime);

                if (MonsterViewModel.TraceTarget != null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }
                else
                {
                    if (!IsPatrolMonster) return;

                    _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, _PatrolDelay);
                    if (_time >= _PatrolDelay)
                    {
                        MonsterViewModel.RequestStateChanged(monsterId, State.Walk);
                    }
                }
                break;
            case State.Walk:

                agent.speed = Mathf.Lerp(agent.speed, _agentSpeed, 10f * Time.fixedDeltaTime);

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

                if (_monsterState.TraceTarget.position != traceTargetPos)
                {
                    traceTargetPos = _monsterState.TraceTarget.position;
                    agent.SetDestination(traceTargetPos);
                }

                _distanceToTarget = Vector3.Distance(transform.position, MonsterViewModel.TraceTarget.transform.position);

                if (_distanceToTarget <= _distanceToStand + 0.03f)
                {
                    agent.speed = Mathf.Lerp(agent.speed, 0f, 10f * Time.fixedDeltaTime);

                    if(agent.speed <= 0.5f)
                    {
                        agent.speed = 0.1f;
                        MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
                        return;
                    }                    
                }
                else
                {
                    agent.speed = Mathf.Lerp(agent.speed, _agentSpeed, 10f * Time.fixedDeltaTime);
                }

                break;
            case State.Alert:
                if (MonsterViewModel.TraceTarget != null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }
                else
                {
                    _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, AlertStateEndTime);
                    if (_time >= AlertStateEndTime)
                    {
                        MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
                    }
                }
                break;
            case State.Incapacitated:
                _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, SubduedEndTime);
                if (_time >= SubduedEndTime)
                {
                    MonsterViewModel.RequestStateChanged(_monsterId, State.Battle);
                }
                break;
            case State.Battle:
                if (MonsterViewModel.TraceTarget == null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
                    return;
                }

                _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, _circleDelay);
                if(_time >= _circleDelay)
                {
                    if(UnityEngine.Random.Range(0, 2) == 0)
                    {
                        //가만히 있을지
                        _time = 0f;
                    }
                    else
                    {
                        //천천히 주위를 돌것인지
                        MonsterViewModel.RequestStateChanged(monsterId, State.Circling);
                        return;
                    }
                }

                transform.LookAt(MonsterViewModel.TraceTarget);

                if (Vector3.Distance(transform.position, MonsterViewModel.TraceTarget.transform.position) > _distanceToStand + 0.03f)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }

                break;
            case State.Circling:
                if(MonsterViewModel.TraceTarget == null)
                {

                }

                traceTargetPos = _monsterState.TraceTarget.position;

                agent.speed = Mathf.Lerp(agent.speed, _agentSpeed, 10f * Time.fixedDeltaTime);
                _circlingSpeed = Mathf.Lerp(_circlingSpeed, 20f, 10f * Time.fixedDeltaTime);

                _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, _circleTimeRange);
                if (_time >= _circleTimeRange)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
                    return;
                }

                //transform.RotateAround(traceTargetPos, Vector3.up, _circlingDir * _circlingSpeed * Time.fixedDeltaTime);
                
                var VecToTarget = transform.position - traceTargetPos;
                var rotatedPos = Quaternion.Euler(0, _circlingDir * _circlingSpeed * Time.fixedDeltaTime, 0) * VecToTarget;

                agent.Move(rotatedPos - VecToTarget);
                transform.rotation = Quaternion.LookRotation(-rotatedPos);

                animator.SetFloat("CirclingDir", _circlingDir);
                //animator.SetFloat("MoveSpeed", (float)agent.speed / monster_Info.WalkSpeed);
                break;
        }
    }


}
