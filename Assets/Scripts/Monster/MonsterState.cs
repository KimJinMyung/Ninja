using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;
using static UnityEngine.UI.GridLayoutGroup;

public class MonsterState : ActorState
{ 
    protected Monster owner;

    protected Rigidbody rb;

    public MonsterState(Monster owner)
    {
        this.owner = owner;

        rb = owner.GetComponent<Rigidbody>();
    }
}

//기본 상태
public class Monster_IdleState : MonsterState
{
    public Monster_IdleState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner.Init_IdleState();
        owner.Agent.angularSpeed = 1000;
    }

    public override void Update()
    {
        base.Update();
    }
}

//패트롤
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }

    [Header("Patrol 가능 길이")]
    [SerializeField] protected float _distance = 15f;

    public override void Enter()
    {
        base.Enter();
        owner.MoveSpeed = owner.Monster_Info.WalkSpeed;
        owner.StartPos = owner.transform.position;
        RandomPoint();
        owner.Agent.destination = owner.PatrolEndPos;
    }

    public override void Update()
    {
        base.Update();     
    }

    protected virtual void RandomPoint()
    {
        while (owner.PatrolEndPos == owner.transform.position)
        {
            Vector3 randomPoint = owner.transform.position + UnityEngine.Random.insideUnitSphere * _distance;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 1.0f, (1 << NavMesh.GetAreaFromName("Walkable"))))
            {
                owner.PatrolEndPos = hit.position;
                break;
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
        owner.MoveSpeed = owner.Monster_Info.RunSpeed;
        owner.Agent.angularSpeed = 3000;
        owner.Animator.SetBool("ComBatMode", true);
    }

    public override void Update()
    {
        base.Update();
    }
}

//경계태세 
public class Monster_AlertState : MonsterState
{
    public Monster_AlertState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner._Time = 0f;
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
        owner._Time = 0f;
        owner.IsSubdued = true;
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsSubdued = false;
    }
}

//전투 돌입
public class Monster_BattleState : MonsterState
{
    public Monster_BattleState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();

        owner._Time = 0f;
        owner.MoveSpeed = 0f;
        owner.Agent.speed = 0f;
        owner.CircleDelay = Random.Range(2f, 5f);
        owner.Agent.enabled = true;
        owner.Animator.SetBool("Circling", false);
    }
}

//주위를 돌기
public class Monster_CirclingState : MonsterState
{
    public Monster_CirclingState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner._Time = 0f;
        owner.CircleTimeRange = Random.Range(3f, 6f);
        owner.CirclingDir = Random.Range(0, 2) == 0 ? 1 : -1;        
        owner.Agent.ResetPath();
        owner.Animator.SetBool("Circling", true);
    }
}

//공격
public class Monster_AttackState : MonsterState
{
    public Monster_AttackState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner.Animator.SetTrigger("Attack");
    }
}

//데미지 받음
public class Monster_HurtState : MonsterState
{
    public Monster_HurtState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();        
        owner.Agent.destination = default;
        owner.Agent.speed = 0;
        //rb.AddForce(owner.transform.forward * -1f * 10f, ForceMode.Impulse);
    }
}

//사망
public class Monster_DeadState : MonsterState
{
    public Monster_DeadState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner.tag = "Dead";
        owner.gameObject.layer = LayerMask.NameToLayer("Dead");
        //사망 애니메이션 실행
        owner.Animator.SetTrigger("Die");
        //임시
        owner.gameObject.SetActive(false);
        //GameManagerDic에서 제거
        MonsterManager.instance.RemoveMonsters(owner.monsterId);
    }
}