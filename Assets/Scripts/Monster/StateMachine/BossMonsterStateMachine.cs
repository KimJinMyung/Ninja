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

    protected bool isAttackAble;


    public BossMonsterStateMachine(Monster owner)
    {
        this.owner = owner;
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

//�⺻
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

        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;

        //AttackStateMachineName = owner.GetRandomSubStateMachineName(out attackTypeIndex);
        owner.SetAttackMethodIndex(1,0);
        //currentAttackIndex = Random.Range(0, owner.SearchSubStateMachineStates(AttackStateMachineName).Count);

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

        if (!isAttackAble)
        {
            _attackDelayTimer += Time.deltaTime;
            if (_attackDelayTimer >= attackDelay) isAttackAble = true;
        }
        else
        {
            _attackDelayTimer = 0f;
            isAttackAble = false;
            //���� ���� index ���ϱ�
            //���� �������� �������� Attack State ��
            //�׷��� ������ ���� �Ÿ��� ������ ���Ͽ� Run State�� �����Ѵ�

            //owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
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

//���ѱ�
public class BossMonster_TraceState : BossMonsterStateMachine
{
    public BossMonster_TraceState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        Debug.Log($"���� ������Ʈ {owner.MonsterViewModel.MonsterState} : ���� Ÿ�� �ε��� {owner.BossAttackTypeIndex}");

    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(owner.transform.position, target.position);

        if(owner.BossAttackTypeIndex == 1)
        {
            if (distance <= 3f)
            {
                //�޺� ���� ����
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
                return;
            }
        }
        else
        {
            if(distance <= 4f)
            {
                //�ڷ� ũ�� ��������.
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
                return;
            }
            else if(distance <= 6f)
            {
                //�뽬 ����
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
    private float slowDownThreshold = 0.5f; // �ӵ��� �� �� ���ϰ� �Ǹ� ������

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

        isFalling = false;
        isGround = false;
    }

    public override void Update()
    {
        base.Update();

        if (owner.transform.position.y >= maxJumpHeight && !isFalling)
        {
            // �ְ����� �����ϸ� ��� �ӵ��� ���� ���߰� ����
            owner.rb.velocity = new Vector3(owner.rb.velocity.x, slowDownThreshold, owner.rb.velocity.z);
            owner.StartCoroutine(HoverAtPeaak());
        }

        if (isFalling)
        {
            // �ϰ� ���¿����� �߷��� ��ȭ�Ͽ� ������ �������� ��
            owner.rb.AddForce(Vector3.down * gravityMultiplier, ForceMode.Acceleration);

            if (owner.rb.velocity.y == 0f && !isGround)
            {
                isGround = true;
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
                return;
            }
        }
    }

    private IEnumerator HoverAtPeaak()
    {
        owner.rb.isKinematic = true;
        yield return new WaitForSeconds(0.2f); // 0.2�� ���� ���
        owner.rb.isKinematic = false;
        owner.rb.velocity = jumpDirection * backDashPower + Vector3.down * initJumpSpeed; // �ʱ� ����� �ӵ��� �ϰ�
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

//����
public class BossMonster_AttackState : BossMonsterStateMachine
{
    public BossMonster_AttackState(Monster owner) : base(owner) { }

    private Vector3 targetDir;

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

        targetDir = (owner.MonsterViewModel.TraceTarget.position - owner.transform.position).normalized;
        isAttackAble = true;
    }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(owner.transform.position, target.position);
        if (distance <= owner.Agent.stoppingDistance && isAttackAble)
        {
            //���� �����̸� ��뽬�� ������ ����
            //������ ����, ���������� �÷��̾ ���� �̵�
            //�뽬 �����̸� �÷��̾ ���� ����
            //���� ��Ÿ����� �Ÿ��� ������ ����
            //�޺� �����̸� �÷��̾ �ٶ󺸸� ���ư����� ����
            //switch (owner.BossAttackTypeIndex)
            //{
            //    case 0:
            //        //owner.animator.applyRootMotion = true;
            //        owner.rb.isKinematic = false;
            //        owner.Agent.enabled = false;
            //        break;
            //    case 1:
            //        owner.animator.applyRootMotion = true;
            //        owner.rb.isKinematic = true;
            //        owner.Agent.enabled = false;
            //        break;
            //    case 2:
            //        owner.rb.isKinematic = true;
            //        owner.Agent.enabled = false;
            //        break;
            //}

            owner.animator.SetTrigger(hashAttack);
            owner.animator.SetInteger(hashAttackTypeIndex, owner.BossAttackTypeIndex);
            owner.animator.SetInteger(hashAttackIndex, owner.BossCurrentAttackIndex);
            isAttackAble = false;
            return;
        }

        //owner.Agent.Move(targetDir * owner.Agent.speed * Time.deltaTime);
        if(owner.Agent.enabled)
            owner.Agent.SetDestination(target.position);

        Debug.Log($"���� ������Ʈ {owner.MonsterViewModel.MonsterState} : ���� Ÿ�� �ε��� {owner.BossAttackTypeIndex}");

    }
}

//�и�����
public class BossMonster_ParryiedState : BossMonsterStateMachine
{
    public BossMonster_ParryiedState(Monster owner) : base(owner) { }
}

//����ȭ
public class BossMonster_SubduedState : BossMonsterStateMachine
{
    public BossMonster_SubduedState(Monster owner) : base(owner) { }
}

//���� ����
public class BossMonster_HurtState : BossMonsterStateMachine
{
    public BossMonster_HurtState(Monster owner) : base(owner) { }
}

//���
public class BossMonster_DeadState : BossMonsterStateMachine
{
    public BossMonster_DeadState(Monster owner) : base(owner) { }
}