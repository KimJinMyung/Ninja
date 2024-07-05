using ActorStateMachine;
using UnityEngine;

public class BossMonsterStateMachine : ActorState
{
    protected Monster owner;

    public BossMonsterStateMachine(Monster owner)
    {
        this.owner = owner;
    }

    public override void Update()
    {
        base.Update();

        if (owner.MonsterViewModel.MonsterState == State.Die) return;
        if(owner.MonsterViewModel.TraceTarget != null)
        {
            Vector3 dir = owner.MonsterViewModel.TraceTarget.position - owner.transform.position;
            dir.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
            owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRotation, 3f * Time.deltaTime);
        }           
    }
}

//기본
public class BossMonster_IdleState : BossMonsterStateMachine
{
    public BossMonster_IdleState(Monster owner) : base(owner) { }

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