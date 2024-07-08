using ActorStateMachine;
using System.Collections;
using System.Data.Common;
using UnityEngine;
using UnityEngine.AI;

public class BossMonsterStateMachine : ActorState
{
    protected Monster owner;
    protected Transform target;

    protected float MovementValue_x;
    protected float MovementValue_z;

    protected static int hashMoveSpeed_x = Animator.StringToHash("MoveSpeed_X");
    protected static int hashMoveSpeed_z = Animator.StringToHash("MoveSpeed_Z");
    protected static int hashAttack = Animator.StringToHash("Attack");
    protected static int hashAttackTypeIndex = Animator.StringToHash("AttackTypeIndex");
    protected static int hashAttackIndex = Animator.StringToHash("AttackIndex");

    protected static int hashBackJump = Animator.StringToHash("BackJump");
    protected static int hashNextAction = Animator.StringToHash("NextAction");
    protected readonly int hashIncapacitated = Animator.StringToHash("Incapacitated");
    protected readonly int hashTriggerIncapacitate = Animator.StringToHash("Incapacitate");

    protected static int IncapacitatedLayer = LayerMask.NameToLayer("Incapacitated");
    protected static int monsterLayer = LayerMask.NameToLayer("Monster");
    protected static int LockOnTargetLayer = LayerMask.NameToLayer("LockOnTarget");
    protected static int LockOnAbleLayer = LayerMask.NameToLayer("LockOnAble");

    protected bool isAttackAble;

    protected Vector3 SpawnPosition;

    public BossMonsterStateMachine(Monster owner)
    {
        this.owner = owner;
        SpawnPosition = owner.transform.position;
    }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.MonsterState == State.Die) return;

        if (owner.MonsterViewModel != null)
            target = owner.MonsterViewModel.TraceTarget;

        if (target != null && owner.MonsterViewModel.MonsterState != State.Attack)
        {
            Vector3 dir = target.position - owner.transform.position;
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRotation, 3f * Time.deltaTime);

            Vector3 velocity = owner.Agent.velocity;
            MovementValue_x = velocity.x;
            MovementValue_z = velocity.z;
        }
        else
        {
            MovementValue_x = 0;
            MovementValue_z = 0;
        }

        owner.animator.SetFloat(hashMoveSpeed_x, MovementValue_x);
        owner.animator.SetFloat(hashMoveSpeed_z, MovementValue_z);
    }
}

//기본
public class BossMonster_IdleState : BossMonsterStateMachine
{
    public BossMonster_IdleState(Monster owner) : base(owner) { }

    private float _attackDelayTimer;
    private float attackDelay;

    string AttackStateMachineName;

    private float AttackRange;

    public override void Enter()
    {
        base.Enter();
        owner.Agent.stoppingDistance = 0;
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;

        owner.SetAttackMethodIndex(1,0);

        _attackDelayTimer = 0f;
        attackDelay = owner.MonsterViewModel.CurrentAttackMethod.AttackSpeed - Random.Range(0f, owner.MonsterViewModel.CurrentAttackMethod.AttackSpeed);

        AttackRange = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;
    }

    public override void Update()
    {
        base.Update();

        if (target == null)
        {
            target = owner.MonsterViewModel.TraceTarget;
            return;
        }

        Player player = target.GetComponent<Player>();
        if (player.ViewModel.playerState == State.Die)
        {
            owner.Agent.SetDestination(SpawnPosition);
            return;
        }else

        if (!isAttackAble)
        {
            _attackDelayTimer += Time.deltaTime;
            if (_attackDelayTimer >= attackDelay) isAttackAble = true;
        }
        else
        {
            _attackDelayTimer = 0f;
            isAttackAble = false;
            //공격 패턴 index 정하기
            //점프 공격으로 정해지면 Attack State 로
            //그렇지 않으면 공격 거리를 좁히기 위하여 Run State로 변경한다
            if (owner.BossAttackTypeIndex == 0) owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
            else owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Run);
            return;
        }

        float distance = Vector3.Distance(owner.transform.position, target.position);
        
        if (distance > AttackRange)
        {
            owner.Agent.SetDestination(target.position);
        }
        else owner.Agent.ResetPath();
    }

    public override void Exit()
    {
        base.Exit();
    }

}

//뒤쫓기
public class BossMonster_TraceState : BossMonsterStateMachine
{
    public BossMonster_TraceState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(owner.transform.position, target.position);

        if(owner.BossAttackTypeIndex == 1)
        {
            if (distance <= 3f)
            {
                //콤보 공격 시작
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
                return;
            }
        }
        else
        {
            if(distance <= 4f)
            {
                //뒤로 크게 물러난다.
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
                return;
            }
            else if(distance <= 6f)
            {
                //대쉬 공격
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
                return;
            }
        }

        owner.Agent.SetDestination(target.position);
    }

    public override void Exit()
    {
        base.Exit();
    }
}

public class BossMonster_BackDashState : BossMonsterStateMachine
{
    public BossMonster_BackDashState(Monster owner) : base(owner) { }

    private float backDashPower = 10f;
    private float gravityMultiplier = 15f;
    private float initJumpSpeed = 15f;
    private float maxJumpHeight;
    private float slowDownThreshold = 0.5f; // 속도가 이 값 이하가 되면 느리게

    private Vector3 jumpDirection;
    private Transform newtarget;
    private bool isFalling;
    private bool isGround;

    public override void Enter()
    {
        base.Enter();

        owner.Agent.enabled = false;
        owner.animator.applyRootMotion = false;
        owner.rb.velocity = Vector3.zero;
        owner.rb.isKinematic = false;

        maxJumpHeight = owner.transform.position.y + owner.monsterHeight + 2f;

        newtarget = owner.MonsterViewModel.TraceTarget;
        if (newtarget == null) return;
        jumpDirection = (owner.transform.position - (newtarget.position - owner.transform.position).normalized * backDashPower);
        jumpDirection.y = 0f;
        jumpDirection.Normalize();

        owner.rb.AddForce(jumpDirection * backDashPower + Vector3.up * initJumpSpeed, ForceMode.Impulse);
        owner.animator.SetTrigger(hashBackJump);

        isFalling = false;
        isGround = false;
    }

    public override void Update()
    {
        base.Update();

        if (owner.transform.position.y >= maxJumpHeight && !isFalling)
        {
            // 최고점에 도달하면 상승 속도를 거의 멈추게 설정
            owner.rb.velocity = new Vector3(owner.rb.velocity.x, slowDownThreshold, owner.rb.velocity.z);
            owner.StartCoroutine(HoverAtPeaak());
        }

        if (isFalling)
        {
            // 하강 상태에서는 중력을 강화하여 빠르게 내려오게 함
            owner.rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);

            if (owner.rb.velocity.y == 0f && !isGround)
            {
                isGround = true;
                owner.animator.SetTrigger(hashNextAction);
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
                return;
            }
        }
    }

    private IEnumerator HoverAtPeaak()
    {
        owner.rb.isKinematic = true;
        yield return new WaitForSeconds(0.2f); // 0.2초 동안 대기
        owner.rb.isKinematic = false;
        owner.rb.velocity = jumpDirection * backDashPower + Vector3.down * initJumpSpeed; // 초기 방향과 속도로 하강
        isFalling = true;
    }

    public override void Exit()
    {
        base.Exit();

        owner.Agent.enabled = true;
        owner.rb.isKinematic = true;
        owner.animator.applyRootMotion = true;
    }
}

//공격
public class BossMonster_AttackState : BossMonsterStateMachine
{
    public BossMonster_AttackState(Monster owner) : base(owner) { }

    private bool isAttackAble;

    public override void Enter()
    {
        base.Enter();

        owner.animator.applyRootMotion = false;

        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.RunSpeed;

        switch (owner.BossAttackTypeIndex)
        {
            case 0:
                owner.Agent.stoppingDistance = 2.5f;
                break;
            case 1:
                owner.Agent.stoppingDistance = 2.8f;
                break;
            case 2:
                owner.Agent.stoppingDistance = 2.8f;
                break;
        }

        isAttackAble = true;
        owner.attackBox.gameObject.SetActive(true);
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(owner.transform.position, target.position);
        if (distance <= owner.Agent.stoppingDistance && isAttackAble)
        {
            owner.animator.SetTrigger(hashAttack);
            owner.animator.SetInteger(hashAttackTypeIndex, owner.BossAttackTypeIndex);
            owner.animator.SetInteger(hashAttackIndex, owner.BossCurrentAttackIndex);
            isAttackAble = false;
            return;
        }

        if(owner.Agent.enabled)
            owner.Agent.SetDestination(target.position);

    }

    public override void Exit()
    {
        base.Exit();
        owner.attackBox.gameObject.SetActive(false);
    }
}

//패링당함
public class BossMonster_ParryiedState : BossMonsterStateMachine
{
    public BossMonster_ParryiedState(Monster owner) : base(owner) { }
}

//무력화
public class BossMonster_SubduedState : BossMonsterStateMachine
{
    public BossMonster_SubduedState(Monster owner) : base(owner) { }

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

        if (owner.MonsterViewModel.MonsterInfo.HP <= 0)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Die);
            return;
        }

        if (_timer < IncapacitatedRangeTime) _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, IncapacitatedRangeTime);
        else
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);
            return;
        }
    }

    public override void Exit()
    {
        base.Exit();
        if (owner.MonsterViewModel.MonsterState == State.Die) return;

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

//공격 받음
public class BossMonster_HurtState : BossMonsterStateMachine
{
    public BossMonster_HurtState(Monster owner) : base(owner) { }
}

//사망
public class BossMonster_DeadState : BossMonsterStateMachine
{
    public BossMonster_DeadState(Monster owner) : base(owner) { }

    private int _DeadMonsterLayer = LayerMask.NameToLayer("Dead");
    private int _RespawnMonsterLayer = LayerMask.NameToLayer("Monster");

    public override void Enter()
    {
        base.Enter();

        owner.Agent.speed = 0f;
        owner.Agent.destination = default;
        owner.rb.isKinematic = false;
        owner.animator.SetLayerWeight(1, 0);
        MonsterManager.instance.DieMonster(owner);
    }

    public override void Update()
    {
        base.Update();

        if (owner.gameObject.layer != _DeadMonsterLayer)
        {
            owner.gameObject.layer = _DeadMonsterLayer;
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