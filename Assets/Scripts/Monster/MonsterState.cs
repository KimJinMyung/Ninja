using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterState : ActorState
{
    protected Monster owner;

    public MonsterState(Monster owner)
    {
        this.owner = owner;
    }   
}

//기본 상태
public class Monster_IdleState : MonsterState
{
    public Monster_IdleState(Monster owner) : base(owner) { }

}

//패트롤
public class Monster_PatrolState : MonsterState
{
    public Monster_PatrolState(Monster owner) : base(owner) { }
}

//따라가기
public class Monster_TraceState : MonsterState
{
    public Monster_TraceState(Monster owner) : base(owner) { }

}

//경계태세 
public class Monster_AlertState : MonsterState
{
    public Monster_AlertState(Monster owner) : base(owner) { }

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