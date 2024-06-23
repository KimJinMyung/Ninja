using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ActorStateMachine;

public class MonsterState : ActorState
{ 
    protected Monster owner;

    public MonsterState(Monster owner)
    {
        this.owner = owner;
    }
}

//�⺻ ����
public class Monster_IdleState : MonsterState
{
    public Monster_IdleState(Monster owner) : base(owner) { }

    private float _PatrolDelay;
    private float _time;

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
}

//��Ʈ��
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }

    private Vector3 patrolEndPos;
    private Vector3 StartPos;

    public override void Enter()
    {
        base.Enter();
        //StartPos = owner.transform.position;
        //patrolEndPos = StartPos;

        //RandomPoint();
    }

    public override void Update()
    {
        base.Update();     
    }

}

//���󰡱�
public class Monster_TraceState : MonsterState
{
    public Monster_TraceState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
}

//����¼� 
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

//����ȭ ����
public class Monster_SubduedState : MonsterState
{
    public Monster_SubduedState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner.IsSubdued = true;
    }

    public override void Exit()
    {
        base.Exit();
        owner.IsSubdued = false;
    }
}

//���� ����
public class Monster_BattleState : MonsterState
{
    public Monster_BattleState(Monster owner) : base(owner) { }
}

//������ ����
public class Monster_CirclingState : MonsterState
{
    public Monster_CirclingState(Monster owner) : base(owner) { }
}

//����
public class Monster_AttackState : MonsterState
{
    public Monster_AttackState(Monster owner) : base(owner) { }

}

//������ ����
public class Monster_HurtState : MonsterState
{
    public Monster_HurtState(Monster owner) : base(owner) { }

}

//���
public class Monster_DeadState : MonsterState
{
    public Monster_DeadState(Monster owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();

        //����� ��Ȱ��ȭ
        owner.gameObject.SetActive(false);
        //GameManagerDic���� ����
        MonsterManager.instance.RemoveMonsters(owner.monsterId);
    }
}