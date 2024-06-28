using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;
using System.Drawing;

public class MonsterState : ActorState
{ 
    protected Monster owner;

    protected int monsterId;

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
        owner.animator.SetBool("ComBatMode", owner.MonsterViewModel.TraceTarget != null);
    }
}

//�⺻ ����
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
                //owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Walk);
                return;
            }
        }
    }
}

//��Ʈ��
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }

    [Header("Patrol ���� ����")]
    protected float _distance;

    private Vector3 StartPos;
    private Vector3 PatrolEndPos;

    public override void Enter()
    {
        base.Enter();
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
        while (true)
        {
            Vector3 randomPoint = owner.transform.position + Random.insideUnitSphere * _distance;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, (1 << NavMesh.GetAreaFromName("Walkable"))))
            {
                return hit.position;
            }
        }
    }
}

//����¼� 
public class Monster_AlertState : MonsterState
{
    public Monster_AlertState(Monster owner) : base(owner) { }

    private float _time;
    private float AlertStateEndTime;

    public override void Enter()
    {
        base.Enter();
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
        }
        else
        {
            _time = Mathf.Clamp(_time + Time.fixedDeltaTime, 0f, AlertStateEndTime);
            if (_time >= AlertStateEndTime)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
            }
        }
    }

}

//����ȭ ����
public class Monster_SubduedState : MonsterState
{
    public Monster_SubduedState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Exit()
    {
        base.Exit();
    }
}

//���� ����
public class Monster_BattleState : MonsterState
{
    public Monster_BattleState(Monster owner) : base(owner) { }

    private float _time;
    private float _circleDelay;

    public override void Enter()
    {
        base.Enter();
        _time = 0;
        _circleDelay = Random.Range(2f, 5f);
        owner.Agent.speed = 0;
        owner.Agent.stoppingDistance = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;
        owner.animator.SetBool("Circling", false);
    }

    public override void Update()
    {
        base.Update();
        if (owner.MonsterViewModel.TraceTarget == null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
            return;
        }

        //Debug.Log($"{owner.MonsterViewModel.MonsterState} {owner.Agent.remainingDistance}/{owner.Agent.stoppingDistance + 1.5f}");

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
                //������ ������
                _time = 0f;
            }
            else
            {
                //õõ�� ������ ��������
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Circling);
                return;
            }
        }

        Vector3 direction = owner.MonsterViewModel.TraceTarget.position - owner.transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        owner.transform.rotation = Quaternion.Slerp(owner.transform.rotation, targetRotation, 3f * Time.deltaTime);

        //owner.transform.LookAt(owner.MonsterViewModel.TraceTarget);
    }
}

//���󰡱�
public class Monster_TraceState : MonsterState
{
    public Monster_TraceState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
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

//������ ����
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

        if (owner.Agent.remainingDistance > owner.Agent.stoppingDistance + 1.5f)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Run);
            return;
        }
        //Debug.Log($"{owner.MonsterViewModel.MonsterState} {owner.Agent.remainingDistance}/{owner.Agent.stoppingDistance + 1.5f}");

        _time = Mathf.Clamp(_time + Time.deltaTime, 0f, _circleTimeRange);
        if(_time >= _circleTimeRange)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
            return;
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();

        var VecToTarget = owner.transform.position - owner.MonsterViewModel.TraceTarget.position;
        var rotatedPos = Quaternion.Euler(0, _circlingDir * _circlingSpeed * Time.fixedDeltaTime, 0) * VecToTarget;

        owner.Agent.Move(rotatedPos - VecToTarget);
        owner.transform.rotation = Quaternion.LookRotation(-rotatedPos);
    }
}

//����
public class Monster_AttackState : MonsterState
{
    public Monster_AttackState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        //�ӽ�
        //���� ��Ÿ��� �޾ƿ;��Ѵ�.
        owner.Agent.stoppingDistance = owner.MonsterViewModel.CurrentAttackMethod.AttackRange;
        owner.animator.SetTrigger("Attack");
    }
}

//������ ����
public class Monster_HurtState : MonsterState
{
    public Monster_HurtState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }
}

//���
public class Monster_DeadState : MonsterState
{
    public Monster_DeadState(Monster owner) : base(owner) { }

    private int _DeadMonsterLayer = LayerMask.NameToLayer("Dead");
    private int _RespawnMonsterLayer = LayerMask.NameToLayer("Monster");

    public override void Enter()
    {
        base.Enter();
        owner.animator.SetBool("Dead", true);
        owner.animator.SetTrigger("Die");
        owner.Agent.speed = 0f;
        owner.Agent.destination = default;
        owner.rb.isKinematic = false;
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

        owner.gameObject.layer = _RespawnMonsterLayer;
        owner.animator.SetBool("Dead", false);
    }
}