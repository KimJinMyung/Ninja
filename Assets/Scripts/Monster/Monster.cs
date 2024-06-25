using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;
using Cinemachine;
using static UnityEngine.UI.GridLayoutGroup;
using System.Collections;
using System.Buffers;
using System.Reflection;
using Player_State.Extension;

public enum monsterType
{
    monster_A,
    monster_B,
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
    public Animator Animator { get { return animator; } }
    protected Rigidbody rb;

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
        Debug.Log(_monsterId);

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

    protected float _time;
    public float _Time { get { return _time; } set { _time = value; } }
    private float _PatrolDelay;
    public float PatrolDelay { get => _PatrolDelay; set { _PatrolDelay = value; } }

    public void Init_IdleState()
    {
        _time = 0f;
        _PatrolDelay = UnityEngine.Random.Range(0.2f, 3f);
        agent.speed = monster_Info.WalkSpeed;
        animator.SetBool("ComBatMode", false);
    }

    public bool IsCurrentState(State state)
    {
        return _monsterState.MonsterState == state;
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
        _monsterState.RequestMonsterInfoChanged(monsterId, monster_Info);   

        SetAttackMethod(monster);
    }

    protected virtual void SetAttackMethod(Monster_data monster)
    {
        var attakList = monster.AttackMethodName;
        if (attakList.Count > 0)
        {
            foreach(string attackName in attakList)
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
        MonsterAI();                

        animator.SetFloat("MoveSpeed", (float)agent.speed / MoveSpeed);
    }

    protected virtual void Update()
    {
        //Debug.Log(_monsterState.MonsterState);
        Debug.Log($"{monsterId} : {monster_Info.HP}");

        if (_monsterState.TraceTarget != null)
        {
            CombatMovementTimer += Time.deltaTime;
        }
    }

    protected virtual void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_monsterState.MonsterState):
                _stateMachine.ChangeState(_monsterState.MonsterState);
                //NewStateEnter(_monsterState.MonsterState);
                break;
            case nameof(_monsterState.TraceTarget):
                if (_monsterState.TraceTarget == null) animator.SetBool("ComBatMode", true);
                else animator.SetBool("ComBatMode", false);
                break;
        }
    }

    protected float _moveSpeed;
    public float MoveSpeed { get { return _moveSpeed; } set { _moveSpeed = value; } }
    #region PatrolStates
    protected Vector3 patrolEndPos;
    public Vector3 PatrolEndPos { get { return patrolEndPos; } set { patrolEndPos = value; } }
    protected Vector3 startPos;
    public Vector3 StartPos { get { return startPos; } set {  startPos = value; } }
    #endregion
    #region Trace
    [Header("Trace 여유 범위")]
    [SerializeField] protected float _attackRange = 0f;
    public float AttackRange { get { return _attackRange; } }
    protected float _distanceToTarget;
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
    public float CircleDelay { get { return _circleDelay; } set { _circleDelay = value; } }
    #endregion
    #region Circling
    protected float _circleTimeRange;
    public float CircleTimeRange { get { return _circleTimeRange; } set { _circleTimeRange = value; } }
    protected float _circlingSpeed;
    protected int _circlingDir = 1;
    public int CirclingDir { get { return _circlingDir; } set { _circlingDir = value; } }
    #endregion
    #region AttackState
    public float CombatMovementTimer;
    public float AttackDelay { get; protected set; }
    #endregion

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

                agent.speed = Mathf.Lerp(agent.speed, MoveSpeed, 10f * Time.fixedDeltaTime);

                if (MonsterViewModel.TraceTarget != null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }
                else
                {
                    if (patrolEndPos != startPos && Vector3.Distance(transform.position, patrolEndPos) <= 0.5f)
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

                Vector3 traceTargetPos = _monsterState.TraceTarget.position;

                agent.SetDestination(traceTargetPos);

                _distanceToTarget = Vector3.Distance(transform.position, MonsterViewModel.TraceTarget.transform.position);

                Debug.Log(_attackRange);
                if (_distanceToTarget <= _attackRange + 10f + 0.03f)
                {
                    agent.speed = Mathf.Lerp(agent.speed, 1f, 10f * Time.deltaTime);

                    if (_distanceToTarget <= _attackRange + 0.03f)
                    {
                        agent.speed = 0f;
                        MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
                        return;
                    }
                }
                else
                {
                    agent.speed = Mathf.Lerp(agent.speed, MoveSpeed, 10f * Time.deltaTime);
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

                if (Vector3.Distance(transform.position, MonsterViewModel.TraceTarget.transform.position) > _attackRange +3f + 0.03f)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
                }

                break;
            case State.Circling:
                if(MonsterViewModel.TraceTarget == null)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
                    return;
                }

                agent.speed = Mathf.Lerp(agent.speed, MoveSpeed, 10f * Time.fixedDeltaTime);
                _circlingSpeed = Mathf.Lerp(_circlingSpeed, 20f, 10f * Time.fixedDeltaTime);

                _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, _circleTimeRange);
                if (_time >= _circleTimeRange)
                {
                    MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
                    return;
                }

                //transform.RotateAround(traceTargetPos, Vector3.up, _circlingDir * _circlingSpeed * Time.fixedDeltaTime);
                
                var VecToTarget = transform.position - _monsterState.TraceTarget.position;
                var rotatedPos = Quaternion.Euler(0, _circlingDir * _circlingSpeed * Time.fixedDeltaTime, 0) * VecToTarget;

                agent.Move(rotatedPos - VecToTarget);
                transform.rotation = Quaternion.LookRotation(-rotatedPos);

                animator.SetFloat("CirclingDir", _circlingDir);
                //animator.SetFloat("MoveSpeed", (float)agent.speed / monster_Info.WalkSpeed);
                break;
            case State.Attack:

                break;            
        }
    }

    public void OnAnimation_AttackEnd()
    {
        MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
        CombatMovementTimer = 0f;
    }


    public void Hurt(int monsterid,Player attacker)
    {
        if (monsterid != this.monsterId) return;

        monster_Info.HP -= attacker.InputVm.player_Data.ATK;

        _monsterState.RequestMonsterInfoChanged(monsterId, monster_Info);

        if (_monsterState.TraceTarget == null) _monsterState.RequestTraceTargetChanged(monsterId, attacker.transform);

        if(monster_Info.HP > 0)
        {
            _monsterState.RequestStateChanged(monsterId, State.Hurt);
            animator.SetTrigger("Hurt");
            agent.enabled = false;

            if(rb == null) rb = gameObject.AddComponent<Rigidbody>();

            Vector3 dir = (attacker.transform.position - transform.position).normalized;
            rb.AddForce(dir * 1f, ForceMode.Impulse);

            StartCoroutine(RemoveRigidbody(rb));
        }
        else
        {
            _monsterState.RequestStateChanged(monsterId, State.Die);
            attacker.InputVm.RequestLockOnTarget(null);
        }
    }

    IEnumerator RemoveRigidbody(Rigidbody rb)
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            if (_monsterState.MonsterState != State.Hurt) break;
        }

        rb.velocity = Vector3.zero;
        Destroy(rb);
        agent.enabled = true;
        yield break;
    }
}
