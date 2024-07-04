using ActorStateMachine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEditor.Animations;
using UnityEngine;
using UnityEngine.AI;
using static Monster;
using static UnityEngine.UI.GridLayoutGroup;
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
    public monsterType Type { get { return type; } }

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

    private Monster_data _initMonsterData;
    public Monster_data InitMonsterData { get { return _initMonsterData; } }

    private Monster_Status_ViewModel _monsterState;
    public Monster_Status_ViewModel MonsterViewModel { get { return _monsterState; } }

    private List<Monster_Attack> monsterAttackMethodList = new List<Monster_Attack>();

    private StateMachine _monsterStateMachine;
    public StateMachine MonsterStateMachine {  get { return _monsterStateMachine; } }

    public int monsterId { get; private set; }

    public Rigidbody rb {  get; private set; }
    public NavMeshAgent Agent { get; private set; }
    public Animator animator { get; private set; }

    public AttackBox attackBox {  get; private set; }

    private float KnockBackDuration = 0.2f;

    protected readonly int hashDead = Animator.StringToHash("Dead");
    protected readonly int hashDie = Animator.StringToHash("Die");

    public float monsterHeight {  get; private set; }

    private void Awake()
    {
        Agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        attackBox = GetComponentInChildren<AttackBox>();

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
        _monsterStateMachine.AddState(State.RetreatAfterAttack, new Monster_RetreatAfterAttackState(this));
        _monsterStateMachine.AddState(State.Hurt, new Monster_HurtState(this));
        _monsterStateMachine.AddState(State.Incapacitated, new Monster_SubduedState(this));
        _monsterStateMachine.AddState(State.Parried, new Monster_ParryiedState(this));
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
        attackBox.gameObject.SetActive(false);

        MonsterManager.instance.SpawnMonster(this);

        monsterHeight = GetComponent<CapsuleCollider>().height;

        animator.SetLayerWeight(1, 1);
    }

    private void OnDisable()
    {
        if (_monsterState != null)
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

    public float CombatMovementTimer {  get; set; }

    private void ReadData_MonsterInfo(monsterType type)
    {
        var monster = DataManager.Instance.GetMonsterData((int)type);
        if (monster == null) return;

        _initMonsterData = monster.MonsterDataClone();

        _monsterState.RequestMonsterInfoChanged(monsterId, monster.MonsterDataClone());

        ChangedCharacterMesh(type);
        UpdateAttackMethod_Data(monster);
    }

    private void ChangedCharacterMesh(monsterType type)
    {
        monsterMesh[type].mesh.SetActive(true);
        animator.runtimeAnimatorController = monsterMesh[type].animation_controller;
        FindStateMachines();
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

        if(_monsterState.TraceTarget != null)
        {
            CombatMovementTimer += Time.deltaTime;
        }

    }

    

    private void FixedUpdate()
    {
        _monsterStateMachine.OnFixedUpdate();
    }

    //두개 이상의 공격 방식을 가지고 있다면
    //플레이어와의 거리가 가까우면 근접으로 멀면 원거리로 변경
    private void UpdateAttackMethod_Data(Monster_data monster)
    {
        if(monsterAttackMethodList.Count > 0) monsterAttackMethodList.Clear();

        var attakList = monster.AttackMethodName;
        if (attakList.Count > 0)
        {
            foreach (string attackName in attakList)
            {
                var attack = DataManager.Instance.GetAttackMethodName(attackName);
                monsterAttackMethodList.Add(attack.Clone());                
            }
        }

        //AttackRange가 작고 AttackSpeed가 빠른 순서대로 나열
        monsterAttackMethodList = monsterAttackMethodList.OrderByDescending(e => e.AttackRange).ThenBy(e => e.AttackSpeed).ToList();
        _monsterState.RequestAttackMethodChanged(monsterId, monsterAttackMethodList, this);
        ChangedWeaponsMesh();
        ChangedAttackStateMachine(_monsterState.CurrentAttackMethod.DataName);
    }    

    //사용하고 있는 무기
    private void ChangedWeaponsMesh()
    {
        foreach(var weapon in monsterWeapons)
        {
            string AttackMethodName = _monsterState.CurrentAttackMethod.DataName.Replace("\"","");
            string CurrentWeaponsType = Enum.GetName(typeof(WeaponsType), weapon.WeaponsType);

            if (AttackMethodName == CurrentWeaponsType)
            {
                weapon.weaponMesh.SetActive(false);
                continue;
            }
            weapon.weaponMesh.SetActive(true);
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
       if (_monsterState.MonsterState == State.Die) return;

        _monsterState.MonsterInfo.HP -= damage;

        if(_monsterState.MonsterInfo.HP > 0)
        {
            ApplyKnockBack(attacker.transform.position);
            _monsterState.RequestTraceTargetChanged(monsterId, attacker.transform);
            _monsterState.RequestStateChanged(monsterId, State.Hurt);
        }
        else
        {
            animator.SetBool(hashDead, true);
            animator.SetTrigger(hashDie);
            _monsterState.RequestStateChanged(monsterId, State.Die);

            if (attacker.ViewModel.LockOnTarget == transform)
            {
                attacker.ViewModel.RequestLockOnTarget(null);
            }
        }
    }

    private void ApplyKnockBack(Vector3 attakerPosition)
    {
        rb.isKinematic = false;
        animator.SetBool("IsKinematic", false);
        KnockBackDuration = 0.2f;

        Vector3 knockbackDir = transform.position - attakerPosition;
        knockbackDir.y = 0;
        knockbackDir.Normalize();

        float knockbackForce = 10f;
        rb.AddForce(knockbackForce * knockbackDir, ForceMode.Impulse);

        animator.SetFloat("HurtDir_z", knockbackDir.z);
        animator.SetFloat("HurtDir_x", knockbackDir.x);
        animator.SetTrigger("Hurt");
    }

    private void KnockBackEnd()
    {
        if (_monsterState.MonsterState == State.Die) return;

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

    private List<AnimatorStateMachine> monsterStateMachines = new List<AnimatorStateMachine>();
    
    private List<AnimatorStateMachine> _currentAttackStateMachine = new List<AnimatorStateMachine>();
    public List<AnimatorStateMachine> CurrentAttackStateMachine { get { return _currentAttackStateMachine; } }

    private void FindStateMachines()
    {
        if (animator == null) return;

        var runtimeAnimatorController = animator.runtimeAnimatorController as AnimatorController;
        if (runtimeAnimatorController == null) return ;

        monsterStateMachines.Clear();

        foreach (var layer in runtimeAnimatorController.layers)
        {
            foreach (var stateMachine in layer.stateMachine.stateMachines)
            {
                monsterStateMachines.Add(stateMachine.stateMachine);
            }
        }
    }

    private int FindSubStateMachineIndex(string subStateMachineName)
    {
        if (animator == null) return -1;

        var runtimeAnimatorController = animator.runtimeAnimatorController as AnimatorController;
        if (runtimeAnimatorController == null) return -1;

        foreach (var layer in runtimeAnimatorController.layers)
        {
            for (int i = 0; i < layer.stateMachine.stateMachines.Length; i++)
            {
                if (layer.stateMachine.stateMachines[i].stateMachine.name == subStateMachineName)
                {
                    return i;
                }
            }
        }

        return -1; // Not found
    }

    private void ChangedAttackStateMachine(string attackMethodName)
    {
        int subStateMachineIndex = FindSubStateMachineIndex(attackMethodName);

        if (subStateMachineIndex != -1)
        {
            _currentAttackStateMachine.Clear();

            foreach(var stateMachine in monsterStateMachines[subStateMachineIndex].stateMachines)
            {
                _currentAttackStateMachine.Add(stateMachine.stateMachine);
            }            
        }
    }

    public int AttackComboIndex_Random()
    {
        int comboIndex = UnityEngine.Random.Range(0, _currentAttackStateMachine.Count);

        return comboIndex;
    }
    
    public void Parried(Player attacker)
    {
        float addParriedPower;
        if (this.type != monsterType.Boss) addParriedPower = 50f;
        else addParriedPower = 1f;

        _monsterState.MonsterInfo.Stamina -= attacker.Player_Info.Strength * addParriedPower;

        _monsterState.RequestStateChanged(monsterId, State.Parried);
    }
}
