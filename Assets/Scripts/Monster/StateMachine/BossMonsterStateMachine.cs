using ActorStateMachine;
using System.Data.Common;
using UnityEditor;
using UnityEngine;

public class BossMonsterStateMachine : ActorState
{
    protected Monster owner;
    protected Transform target;

    protected float MovementValue_x;
    protected float MovementValue_z;

    protected static int hashMoveSpeed_x = Animator.StringToHash("MoveSpeed_X");
    protected static int hashMoveSpeed_z = Animator.StringToHash("MoveSpeed_Z");

    protected bool isAttackAble;

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

//기본
public class BossMonster_IdleState : BossMonsterStateMachine
{
    public BossMonster_IdleState(Monster owner) : base(owner) { }

    private float Movespeed;
    private float currentSpeed;

    private int AttackCount;

    private float _attackDelayTimer;
    private float attackDelay;

    string AttackStateMachineName;

    public override void Enter()
    {
        base.Enter();

        Movespeed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;
        owner.Agent.stoppingDistance = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;

        AttackCount = owner.CurrentAttackStateMachine.Count;

        AttackStateMachineName = owner.GetRandomSubStateMachineName();

        Debug.Log("1. : "+AttackStateMachineName);
        foreach(var item in owner.SearchSubStateMachineStates(AttackStateMachineName))
        {
            Debug.Log("2. : "+ item.name);
        }

        _attackDelayTimer = 0f;
        attackDelay = owner.MonsterViewModel.CurrentAttackMethod.AttackSpeed;
    }

    public override void Update()
    {
        base.Update();

        if(target != null)
        {
            if (owner.Agent.remainingDistance > owner.Agent.stoppingDistance + 0.2f) currentSpeed = Movespeed;
            else
            {
                //디버그용
                currentSpeed = 0f;
                
                if(isAttackAble)
                {
                    isAttackAble = false;
                    //공격 패턴 index 정하기
                    //점프 공격으로 정해지면 Attack State 로
                    //그렇지 않으면 공격 거리를 좁히기 위하여 Run State로 변경한다
                    
                }
            }

            //MovementValue = Mathf.Lerp(MovementValue, currentSpeed, 3 * Time.deltaTime);
            //if (MovementValue <= 0.1f) MovementValue = 0f;

            owner.Agent.speed = currentSpeed;
            owner.Agent.SetDestination(target.position);
        }
    }

}

//뒤쫓기
public class BossMonster_TraceState : BossMonsterStateMachine
{
    public BossMonster_TraceState(Monster owner) : base(owner) { }
}

//공격
public class BossMonster_AttackState : BossMonsterStateMachine
{
    public BossMonster_AttackState(Monster owner) : base(owner) { }
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
}