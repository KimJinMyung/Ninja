using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class MonsterState : ActorState
{
    protected Monster owner;
    protected float _attackDelay;
    protected int monsterId;

    public MonsterState(Monster owner)
    {
        this.owner = owner;
        _attackDelay = owner.AttackDelay;
    }

    public override void Enter()
    {
        base.Enter();
        monsterId = owner.monsterId;
    }

    public override void Update()
    {
        base.Update();
        if(owner.MonsterViewModel.MonsterInfo.HP <= 0)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Die);            
        }
    }
}

//기본 상태
public class Monster_IdleState : MonsterState
{
    public Monster_IdleState(Monster owner) : base(owner) { }

    private float _PatrolDelay;
    private float _time;

    public override void Enter()
    {
        base.Enter();
        _time = 0f;
        _PatrolDelay = Random.Range(0.2f, 3f);
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.WalkSpeed;
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.MonsterState == State.Die) return;

        if(owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
        }else
        {
            if (!owner.IsPatrolMonster) return;

            _time = Mathf.Clamp(_time + Time.deltaTime, 0f, _PatrolDelay);
            if(_time >= _PatrolDelay)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Walk);
            }
        }
    }
}

//패트롤
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }

    private Vector3 patrolEndPos;
    private Vector3 StartPos;

    public override void Enter()
    {
        base.Enter();
        StartPos = owner.transform.position;
        patrolEndPos = StartPos;
        //if (owner.IsPatrolMonster)
        //{
        //    if (owner.IsRandomPatrolMonster)
        //    {
        //        RandomPoint();
        //    }
        //    else
        //    {
        //        //패트롤 지점 지정
        //    }
        //}
        RandomPoint();
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
        }
        else
        {
            if (patrolEndPos != StartPos && Vector3.Distance(owner.transform.position, patrolEndPos) <= 0.2f)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
            }
        }        
    }

    private void RandomPoint()
    {
        while(patrolEndPos == owner.transform.position)
        {
            Vector3 randomPoint = owner.transform.position + Random.insideUnitSphere * 15f;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, (1 << NavMesh.GetAreaFromName("Walkable"))))
            {
                patrolEndPos = hit.position;
                break;
            }
        }        
    }

}

//따라가기
public class Monster_TraceState : MonsterState
{
    public Monster_TraceState(Monster owner) : base(owner) { }

    private Transform target;
    public override void Enter()
    {
        base.Enter();
        target = owner.MonsterViewModel.TraceTarget;
        owner.Agent.speed = owner.MonsterViewModel.MonsterInfo.RunSpeed;
    }

    public override void Update()
    {
        base.Update();

        if(owner.MonsterViewModel.TraceTarget == null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Alert);
            return;
        }

        owner.Agent.SetDestination(target.position);
    }
}

//경계태세 
public class Monster_AlertState : MonsterState
{
    public Monster_AlertState(Monster owner) : base(owner) { }

    private float AlertStateEndTime = 2f;
    private float _timer;

    public override void Enter()
    {
        base.Enter();
        _timer = 0;
    }

    public override void Update()
    {
        base.Enter();

        if (owner.MonsterViewModel.TraceTarget != null)
        {
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Trace);
        }
        else
        {
            _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, AlertStateEndTime);
            if(_timer >= AlertStateEndTime)
            {
                owner.MonsterViewModel.RequestStateChanged(monsterId, State.Idle);
            }
        }
    }

}

//무력화 상태
public class Monster_SubduedState : MonsterState
{
    public Monster_SubduedState(Monster owner) : base(owner) { }
}

//전투 돌입
public class Monster_BattleState : MonsterState
{
    public Monster_BattleState(Monster owner) : base(owner) { }
}

//공격
public class Monster_AttackState : MonsterState
{
    public Monster_AttackState(Monster owner) : base(owner) { }

}

//사망
public class Monster_DeadState : MonsterState
{
    public Monster_DeadState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();

        //사망시 비활성화
        owner.gameObject.SetActive(false);
        //GameManagerDic에서 제거
        MonsterManager.instance.RemoveMonsters(owner.monsterId);
    }
}