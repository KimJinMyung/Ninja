using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;

public class MonsterState : ActorState
{ 
    protected Monster owner;

    protected int monsterId;
    protected float MovementValue;

    protected readonly int hashParried = Animator.StringToHash("Parried");
    protected readonly int hashParry = Animator.StringToHash("Parry");
    protected readonly int hashDead = Animator.StringToHash("Dead");
    protected readonly int hashIncapacitated = Animator.StringToHash("Incapacitated");
    protected readonly int hashTriggerIncapacitate = Animator.StringToHash("Incapacitate");

    protected static int IncapacitatedLayer = LayerMask.NameToLayer("Incapacitated");
    protected static int monsterLayer = LayerMask.NameToLayer("Monster");
    protected static int LockOnTargetLayer = LayerMask.NameToLayer("LockOnTarget");
    protected static int LockOnAbleLayer = LayerMask.NameToLayer("LockOnAble");
    
    public MonsterState(Monster owner)
    {
        this.owner = owner;     
    }

    public override void Enter()
    {
        base.Enter();
        monsterId = owner.monsterId;
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel == null) return;

        owner.animator.SetBool("ComBatMode", owner.MonsterViewModel.TraceTarget != null);

        owner.animator.SetFloat("MoveSpeed", MovementValue);
    }
}

//기본 상태
public class Monster_IdleState : MonsterState
{
    public Monster_IdleState(Monster owner) : base(owner) { }

    private float _timer;
    private float _patrolDelay;

    public override void Enter()
    {
        base.Enter();
        _timer = 0f;
        _patrolDelay = Random.Range(2f, 5f);
        owner.Agent.speed = 0f;
        owner.Agent.angularSpeed = 1000;
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Battle);
            return;
        }
        else
        {
            _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, _patrolDelay);
            if(_timer >= _patrolDelay)
            {
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Walk);
                return;
            }
        }
    }
}

//패트롤
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }

    [Header("Patrol 가능 길이")]
    protected float _distance;

    private Vector3 StartPos;
    private Vector3 PatrolEndPos;

    public override void Enter()
    {
        base.Enter();
        MovementValue = 1f;
        _distance = owner.MonsterViewModel.MonsterInfo.ViewRange;
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;
        owner.Agent.stoppingDistance = 0f;
        PatrolEndPos = RandomPoint();
        StartPos = owner.transform.position;
    }

    public override void Update()
    {
        base.Update();        

        if (owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
            return;
        }
        else
        {
            owner.Agent.SetDestination(PatrolEndPos);

            if (StartPos != PatrolEndPos && Vector3.Distance(owner.transform.position, PatrolEndPos) <= 0.5f)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
                return;
            }
        }        
    }

    protected virtual Vector3 RandomPoint()
    {
        int maxAttempts = 30;
        int attempts = 0;

        while (attempts < maxAttempts)
        {
            Vector3 randomPoint = owner.transform.position + Random.insideUnitSphere * _distance;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, (1 << NavMesh.GetAreaFromName("Walkable"))))
            {
                return hit.position;
            }
            attempts++;
        }

        return owner.transform.position;
    }
}

//경계태세 
public class Monster_AlertState : MonsterState
{
    public Monster_AlertState(Monster owner) : base(owner) { }

    private float _time;
    private float AlertStateEndTime;

    public override void Enter()
    {
        base.Enter();
        MovementValue = 0;
        _time = 0.0f;
        owner.Agent.speed = 0f;
        AlertStateEndTime = Random.Range(2f, 3f);
    }

    public override void Update()
    {
        base.Enter();
        if (owner.MonsterViewModel.TraceTarget != null)
        {
            if (owner.Agent.remainingDistance > owner.Agent.stoppingDistance + 1.5f)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Run);
                return;
            }
            else
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
                return;
            }
        }
        else
        {
            _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, AlertStateEndTime);
            if (_time >= AlertStateEndTime)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
                return;
            }
        }
    }

}

//패링 당함
public class Monster_ParryiedState : MonsterState
{
    public Monster_ParryiedState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner.animator.SetTrigger(hashParried);
    }

    public override void Exit()
    {
        base.Exit();

        if (owner.MonsterViewModel.MonsterState != State.Incapacitated) owner.animator.SetBool(hashParry, false);
    }
}

//무력화 상태
public class Monster_SubduedState : MonsterState
{
    public Monster_SubduedState(Monster owner) : base(owner) { }

    private float _timer;
    private float IncapacitatedRangeTime = 5f;

    public override void Enter()
    {
        base.Enter();
        owner.gameObject.layer = IncapacitatedLayer;
        owner.animator.SetTrigger(hashTriggerIncapacitate);
        owner.animator.SetBool(hashIncapacitated, true);
        _timer = 0;
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel.MonsterInfo.HP <= 0)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Die);
            return;
        }

        if (_timer < IncapacitatedRangeTime) _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, IncapacitatedRangeTime);
        else
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (owner.MonsterViewModel.MonsterState == State.Die) return;

        owner.animator.SetBool(hashParry, false);
        owner.animator.SetBool(hashIncapacitated, false);

        owner.MonsterViewModel.MonsterInfo.Stamina = owner.InitMonsterData.Stamina;

        if (owner.MonsterViewModel.TraceTarget == null)
        {
            owner.gameObject.layer = monsterLayer;
            return;
        }

        Player_LockOn player = owner.MonsterViewModel.TraceTarget.GetComponent<Player_LockOn>();

        if (player.ViewModel.LockOnTarget == owner.transform)
        {
            owner.gameObject.layer = LockOnTargetLayer;
        }
        else if (player.ViewModel.LockOnAbleTarget)
        {
            owner.gameObject.layer = LockOnAbleLayer;
        }
        else owner.gameObject.layer = monsterLayer;
    }
}

//전투 돌입
public class Monster_BattleState : MonsterState
{
    public Monster_BattleState(Monster owner) : base(owner) { }

    private float _time;
    private float _circleDelay;

    public override void Enter()
    {
        base.Enter();
        MovementValue = 0f;
        _time = 0;
        _circleDelay = Random.Range(2f, 5f);
        owner.Agent.speed = 0;
        owner.Agent.stoppingDistance = owner.MonsterViewModel.CurrentAttackMethod.AttackRange + 1f;
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.TraceTarget == null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
            return;
        }

        if (owner.Agent.remainingDistance > owner.Agent.stoppingDistance + 1.5f)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Run);
            return;
        }

        _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, _circleDelay);
        if (_time >= _circleDelay)
        {
            if (Random.Range(0, 2) == 0)
            {
                //가만히 있을지
                _time = 0f;
            }
            else
            {
                //천천히 주위를 돌것인지
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Circling);
                return;
            }
        }

        Vector3 direction = owner.MonsterViewModel.TraceTarget.position - owner.transform.position;
        direction.y= 0f;
        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, 3f * Time.deltaTime);
    }
}

//따라가기
public class Monster_TraceState : MonsterState
{
    public Monster_TraceState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        MovementValue = 1f;
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.RunSpeed;
        owner.Agent.angularSpeed = 3000f;
        owner.Agent.stoppingDistance = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.TraceTarget == null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
            return;
        }

        owner.Agent.SetDestination(owner.MonsterViewModel.TraceTarget.position);

        if (owner.Agent.remainingDistance < owner.Agent.stoppingDistance + 0.5f)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
            return;
        }
    }
}

//주위를 돌기
public class Monster_CirclingState : MonsterState
{
    public Monster_CirclingState(Monster owner) : base(owner) { }

    private float _time;

    private float _circlingSpeed;
    private float _circleTimeRange;
    private int _circlingDir;

    public override void Enter()
    {
        base.Enter();
        _time = 0f;
        _circlingSpeed = 20f;
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;
        owner.Agent.stoppingDistance = owner.MonsterViewModel.CurrentAttackMethod.AttackRange + 1f;
        _circleTimeRange = Random.Range(3f, 6f);
        _circlingDir = Random.Range(0, 2) == 0 ? 1 : -1;
        owner.animator.SetBool("Circling", true);
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.TraceTarget == null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
            return;
        }

        owner.Agent.destination = owner.MonsterViewModel.TraceTarget.position;

        if (owner.Agent.remainingDistance > owner.Agent.stoppingDistance + 2f)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Run);
            return;
        }

        _time = Mathf.Clamp(_time + Time.deltaTime, 0f, _circleTimeRange);
        if(_time >= _circleTimeRange)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
            return;
        }

        owner.animator.SetFloat("CirclingDir", _circlingDir);
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var VecToTarget = owner.transform.position - owner.MonsterViewModel.TraceTarget.position;
        var rotatedPos = Quaternion.Euler(0, _circlingDir * _circlingSpeed * Time.fixedDeltaTime, 0) * VecToTarget;

        owner.Agent.Move(rotatedPos - VecToTarget);
        owner.transform.rotation = Quaternion.LookRotation(-rotatedPos);
    }

    public override void Exit()
    {
        base.Exit();
        owner.animator.SetBool("Circling", false);
    }
}

//공격
public class Monster_AttackState : MonsterState
{
    public Monster_AttackState(Monster owner) : base(owner) { }

    private float attackRange;
    private bool isAttackAble;

    private int AttackIndex;

    public override void Enter()
    {
        base.Enter();
        owner.animator.applyRootMotion = true;

        isAttackAble = true;
        attackRange = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;
        owner.CombatMovementTimer = 0;
        owner.Agent.stoppingDistance = attackRange - 0.3f;
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.RunSpeed;
        owner.attackBox.gameObject.SetActive(true);

        AttackIndex = owner.AttackComboIndex_Random();

        float Heightdistance = Mathf.Abs(owner.MonsterViewModel.TraceTarget.position.y - owner.transform.position.y);

        while (owner.MonsterViewModel.CurrentAttackMethod.DataName == System.Enum.GetName(typeof(WeaponsType), WeaponsType.DaggerAttack) 
            && Heightdistance > 0.1f 
            && AttackIndex == 1)
        {
            AttackIndex = owner.AttackComboIndex_Random();
        }
    }

    public override void Update()
    {
        base.Update();

        MovementValue = owner.Agent.velocity != Vector3.zero ? 1 : 0;

        Transform target = owner.MonsterViewModel.TraceTarget;

        if(target == null)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Alert);
            return;
        }        

        float distance = Vector3.Distance(owner.transform.position, target.position);

        if(distance > 1.7f)
        {
            owner.Agent.SetDestination(target.position);
        }else owner.animator.applyRootMotion = false;

        if (distance <= attackRange && isAttackAble)
        {
            isAttackAble = false;

            foreach(var state in System.Enum.GetValues(typeof(WeaponsType)))
            {
                if(owner.MonsterViewModel.CurrentAttackMethod.DataName == state.ToString())
                {
                    owner.animator.SetTrigger(state.ToString());
                }
            }            

            owner.animator.SetInteger("AttackIndex", AttackIndex);
        }
    }

    public override void Exit()
    {
        base.Exit();
        owner.attackBox.gameObject.SetActive(false);
    }
}

//공격 후 거리 벌리기
public class Monster_RetreatAfterAttackState : MonsterState
{
    public Monster_RetreatAfterAttackState(Monster owner) : base(owner) { }

    private float attackRange;
    public override void Enter()
    {
        base.Enter();
        MovementValue = -1;

        owner.Agent.speed = - owner.MonsterViewModel.MonsterInfo.WalkSpeed;
        attackRange = owner.MonsterViewModel.CurrentAttackMethod.AttackRange + 1f;
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.TraceTarget == null)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Alert);
            return;
        }

        float distance = Vector3.Distance(owner.transform.position, owner.MonsterViewModel.TraceTarget.position);

        if (distance >= attackRange)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Battle);
            return;
        }

        Vector3 targetDir = owner.MonsterViewModel.TraceTarget.position - owner.transform.position;
        targetDir.y = 0;

        if (owner.MonsterViewModel.CurrentAttackMethod.AttackType == "Short")
        {
            owner.Agent.Move( - targetDir.normalized * attackRange * Time.deltaTime);
        }                
        
        owner.transform.rotation = Quaternion.RotateTowards(owner.transform.rotation, Quaternion.LookRotation(targetDir), 500 * Time.deltaTime);
    }
}

//데미지 받음
public class Monster_HurtState : MonsterState
{
    public Monster_HurtState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }
}

//사망
public class Monster_DeadState : MonsterState
{
    public Monster_DeadState(Monster owner) : base(owner) { }

    private int _DeadMonsterLayer = LayerMask.NameToLayer("Dead");
    private int _RespawnMonsterLayer = LayerMask.NameToLayer("Monster");

    private float _timer;

    public override void Enter()
    {
        base.Enter();
        _timer = 0;
        owner.Agent.speed = 0f;
        owner.Agent.destination = default;
        owner.rb.isKinematic = false;
        owner.animator.SetLayerWeight(1, 0);
        owner.MonsterViewModel.MonsterInfo.Life--;
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel.MonsterInfo.Life > 0)
        {
            if(_timer > 5f)
            {
                owner.Resurrection();
                return;
            }
            else
            {
                _timer += Time.deltaTime;
            }
        }
        else
        {
            if (owner.gameObject.layer != _DeadMonsterLayer)
            {
                owner.gameObject.layer = _DeadMonsterLayer;
            }

            MonsterManager.instance.DieMonster(owner);
        }        
    }

    public override void Exit()
    {
        base.Exit();

        Debug.Log(owner.MonsterViewModel.MonsterState);
        owner.animator.SetLayerWeight(1, 1);
        owner.gameObject.layer = _RespawnMonsterLayer;
        owner.animator.SetBool("Dead", false);
    }
}