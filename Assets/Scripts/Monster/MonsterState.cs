using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;
using static UnityEngine.UI.GridLayoutGroup;

public class MonsterState : ActorState
{ 
    protected Monster owner;

    protected float MovementSpeed;
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
        Debug.Log(_patrolDelay);
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Trace);
        }
        else
        {
            _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, _patrolDelay);
            if(_timer >= _patrolDelay)
            {
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Walk);
            }
        }
    }
}

//패트롤
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }

    [Header("Patrol 가능 길이")]
    [SerializeField] protected float _distance = 15f;

    private Vector3 StartPos;
    private Vector3 PatrolEndPos;

    public override void Enter()
    {
        base.Enter();
        MovementSpeed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;
        PatrolEndPos = RandomPoint();
        StartPos = owner.transform.position;        
    }

    public override void Update()
    {
        base.Update();

        owner.Agent.speed = Mathf.Lerp(owner.Agent.speed, MovementSpeed, 10f * Time.deltaTime);

        Debug.Log($"{PatrolEndPos} / {owner.transform.position}");

        if (owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Run);
        }
        else
        {
            owner.Agent.SetDestination(PatrolEndPos);

            if (StartPos != PatrolEndPos && Vector3.Distance(owner.transform.position, PatrolEndPos) <= 0.5f)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
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

//따라가기
public class Monster_TraceState : MonsterState
{
    public Monster_TraceState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        MovementSpeed = owner.MonsterViewModel.MonsterInfo.RunSpeed;
        owner.Agent.angularSpeed = 3000;
        owner.animator.SetBool("ComBatMode", true);        
    }

    public override void Update()
    {
        base.Update();
        owner.Agent.SetDestination(owner.MonsterViewModel.TraceTarget.position);

        //임시 5f => AttackRange로 교체 예정
        if(Vector3.Distance(owner.transform.position, owner.MonsterViewModel.TraceTarget.position) <= 5f)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
        }
    }
}

//경계태세 
public class Monster_AlertState : MonsterState
{
    public Monster_AlertState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Enter();       
    }

}

//무력화 상태
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

//전투 돌입
public class Monster_BattleState : MonsterState
{
    public Monster_BattleState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }
}

//주위를 돌기
public class Monster_CirclingState : MonsterState
{
    public Monster_CirclingState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }
}

//공격
public class Monster_AttackState : MonsterState
{
    public Monster_AttackState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
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

    public override void Enter()
    {
        base.Enter();
    }
}