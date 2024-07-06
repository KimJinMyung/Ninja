using ActorStateMachine;
using System.Data.Common;
using UnityEditor;
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

    protected bool isAttackAble;

    protected int attackTypeIndex;
    protected int currentAttackIndex;

    public BossMonsterStateMachine(Monster owner)
    {
        this.owner = owner;
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.MonsterState == State.Die) return;

        target = owner.MonsterViewModel.TraceTarget;

        Debug.Log(owner.MonsterViewModel.MonsterState);

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

        AttackStateMachineName = owner.GetRandomSubStateMachineName(out attackTypeIndex);

        currentAttackIndex = Random.Range(0, owner.SearchSubStateMachineStates(AttackStateMachineName).Count);

        _attackDelayTimer = 0f;
        attackDelay = owner.MonsterViewModel.CurrentAttackMethod.AttackSpeed - Random.Range(0f, owner.MonsterViewModel.CurrentAttackMethod.AttackSpeed);

        AttackRange = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;
    }

    public override void Update()
    {
        base.Update();

        if (target == null) return;

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

            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
            return;
            //if (attackTypeIndex == 0) owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Attack);
            //else owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Run);
        }

        float distance = Vector3.Distance(owner.transform.position, target.position);

        if (distance > AttackRange)
        {
            owner.Agent.SetDestination(target.position);
        }
        else owner.Agent.ResetPath();
    }

}

//���ѱ�
public class BossMonster_TraceState : BossMonsterStateMachine
{
    public BossMonster_TraceState(Monster owner) : base(owner) { }

    public override void Update()
    {
        base.Update();

        float distance = Vector3.Distance(owner.transform.position, target.position);

        if(attackTypeIndex == 1)
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
}

public class BossMonster_BackDashState : BossMonsterStateMachine
{
    public BossMonster_BackDashState(Monster owner) : base(owner) { }

    private float backDashPower = 20f;
    private float jumpForce = 20f;

    public override void Enter()
    {
        base.Enter();

        owner.Agent.enabled = false;
        owner.rb.velocity = Vector3.zero;

        Vector3 jumpDir = (owner.transform.position - owner.transform.forward * backDashPower).normalized;
        owner.rb.AddForce(jumpDir * jumpForce + Vector3.up * jumpForce, ForceMode.Impulse);
    }

    public override void Update()
    {
        base.Update();

        
    }

    public override void Exit()
    {
        base.Exit();

        owner.enabled = true;
    }
}

//����
public class BossMonster_AttackState : BossMonsterStateMachine
{
    public BossMonster_AttackState(Monster owner) : base(owner) { }
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