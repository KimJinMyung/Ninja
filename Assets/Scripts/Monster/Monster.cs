using ActorStateMachine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static Monster;
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
    [SerializeField] private monsterType type;

    [Serializable]
    public class MonsterMesh
    {
        public monsterType _monsterType;
        public GameObject mesh;
        public AnimatorController animation_controller;
    }

    [Serializable]
    public class WeaponsMesh
    {
        public WeaponsType WeaponsType;
        public GameObject weaponMesh;
    }

    [SerializeField]
    MonsterMesh[] monsterMeshes;

    [SerializeField]
    WeaponsMesh[] monsterWeapons;

    private Dictionary<monsterType, MonsterMesh> monsterMesh;

    private Monster_Status_ViewModel _monsterState;
    public Monster_Status_ViewModel MonsterViewModel { get { return _monsterState; } }

    private Monster_data monster_Info;

    private List<Monster_Attack> monsterAttackMethodList = new List<Monster_Attack>();

    private StateMachine _monsterStateMachine;
    public StateMachine MonsterStateMachine {  get { return _monsterStateMachine; } }

    public int monsterId { get; protected set; }

    private Rigidbody rb;
    public NavMeshAgent Agent { get; protected set; }
    public Animator animator { get; protected set; }

    private float KnockBackDuration = 0.2f;

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();

        monsterMesh = new Dictionary<monsterType, MonsterMesh>();

        foreach(MonsterMesh monster in monsterMeshes)
        {
            monsterMesh.Add(monster._monsterType, monster);
            monster.mesh.SetActive(false);
        }

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

    private void OnEnable()
    {
        monsterId = this.gameObject.GetInstanceID();

        if(_monsterState == null)
        {
            _monsterState = new Monster_Status_ViewModel();
            _monsterState.PropertyChanged += OnPropertyChanged;
            _monsterState.RegisterMonsterTypeChanged(monsterId, true);
            _monsterState.RegisterStateChanged(monsterId, true);
            _monsterState.RegisterMonsterInfoChanged(monsterId, true);
            _monsterState.RegisterAttackMethodChanged(monsterId, true);
            _monsterState.RegisterTraceTargetChanged(monsterId, true);
        }

        _monsterState.RequestMonsterTypeChanged(monsterId, type);
        _monsterState.RequestStateChanged(monsterId, State.Idle);
        
        ReadData_MonsterInfo(type);

        MonsterManager.instance.SpawnMonster(this);
    }

    private void OnDisable()
    {
        if(_monsterState != null)
        {
            _monsterState.RegisterTraceTargetChanged(monsterId, false);
            _monsterState.RegisterAttackMethodChanged(monsterId, false);
            _monsterState.RegisterMonsterInfoChanged(monsterId, false);
            _monsterState.RegisterStateChanged(monsterId, false);
            _monsterState.RegisterMonsterTypeChanged(monsterId, false);
            _monsterState.PropertyChanged -= OnPropertyChanged;
            _monsterState = null;
        }        
    }

    public float CombatMovementTimer {  get; private set; }

    private void ReadData_MonsterInfo(monsterType type)
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        monster_Info = monster;
        _monsterState.RequestMonsterInfoChanged(monsterId, monster_Info);

        ChangedCharacterMesh(type);
        GetAttackMethod_Data(monster);
    }

    private void ChangedCharacterMesh(monsterType type)
    {
        monsterMesh[type].mesh.SetActive(true);
        animator.runtimeAnimatorController = monsterMesh[type].animation_controller;
    }

    private void Update()
    {
        if(_monsterState != null)
        {
            if(_monsterState.MonsterType != type)
            {
                monsterMesh[_monsterState.MonsterType].mesh.SetActive(false);
                _monsterState.RequestMonsterTypeChanged(monsterId, type);
            }
        }

        UpdateAttackMethod();
        _monsterStateMachine.OnUpdate();
        KnockBackEnd();
        Debug.Log(MonsterViewModel.MonsterState);
        //임시
        Debug.Log($"공격 사거리 : {_monsterState.CurrentAttackMethod.AttackRange}");
    }

    private void FixedUpdate()
    {
        _monsterStateMachine.OnFixedUpdate();
    }

    //두개 이상의 공격 방식을 가지고 있다면
    //플레이어와의 거리가 가까우면 근접으로 멀면 원거리로 변경
    private void GetAttackMethod_Data(Monster_data monster)
    {
        if(monsterAttackMethodList.Count > 0) monsterAttackMethodList.Clear();

        var attakList = monster.AttackMethodName;
        if (attakList.Count > 0)
        {
            foreach (string attackName in attakList)
            {
                var attack = DataManager.Instance.GetAttackMethodName(attackName);
                monsterAttackMethodList.Add(attack);                
            }
        }

        //AttackRange가 작고 AttackSpeed가 빠른 순서대로 나열
        monsterAttackMethodList = monsterAttackMethodList.OrderByDescending(e => e.AttackRange).ThenBy(e => e.AttackSpeed).ToList();
        _monsterState.RequestAttackMethodChanged(monsterId, monsterAttackMethodList, this);
        ChangedWeaponsMesh();
    }    

    private void ChangedWeaponsMesh()
    {
        foreach(var weapon in monsterWeapons)
        {
            if(_monsterState.CurrentAttackMethod.DataName == Enum.GetName(typeof(WeaponsType), weapon.WeaponsType))
            {
                weapon.weaponMesh.SetActive(true);
                continue;
            }
            weapon.weaponMesh.SetActive(false);
        }
    }

    private void UpdateAttackMethod()
    {
        if(_monsterState.TraceTarget == null) return;

        _monsterState.RequestAttackMethodChanged(monsterId, monsterAttackMethodList, this);
    }

    public bool IsCurrentState(State state)
    {
        return _monsterState.MonsterState == state;
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
        switch(e.PropertyName)
        {
            case nameof(_monsterState.MonsterState):
                _monsterStateMachine.ChangeState(_monsterState.MonsterState);
                break;
            case nameof(_monsterState.MonsterType):
                ReadData_MonsterInfo(_monsterState.MonsterType);
                break;
            case nameof(_monsterState.CurrentAttackMethod):
                ChangedWeaponsMesh();
                break;
        }        
    }

    public void Hurt(float damage, Player attacker)  
    {
        monster_Info.HP -= damage;

        //임시
        if(true/*monster_Info.HP > 0*/)
        {
            _monsterState.RequestTraceTargetChanged(monsterId, attacker.transform);
            _monsterState.RequestStateChanged(monsterId, State.Hurt);
            ApplyKnockBack(attacker.transform.position);
        }
        //else
        //{
        //    _monsterState.RequestStateChanged(monsterId, State.Die);
        //    if (attacker.ViewModel.LockOnTarget == transform)
        //    {
        //        attacker.ViewModel.RequestLockOnTarget(null);
        //    }
        //}
        
    }

    private void ApplyKnockBack(Vector3 attakerPosition)
    {
        rb.isKinematic = false;
        animator.SetBool("IsKinematic", false);
        KnockBackDuration = 0.2f;

        Vector3 knockbackDir = (transform.position - attakerPosition).normalized;
        knockbackDir.y = 0;

        float knockbackForce = 50f;
        rb.AddForce(knockbackForce * knockbackDir, ForceMode.Impulse);
    }

    private void KnockBackEnd()
    {
        if (!rb.isKinematic)
        {
            KnockBackDuration -= Time.deltaTime;
            if(KnockBackDuration <= 0)
            {
                rb.isKinematic = true;
                _monsterState.RequestStateChanged(monsterId, State.Battle);
                animator.SetBool("IsKinematic", true);
            }
        }
    }
}
