using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster_AttackEnd : StateMachineBehaviour
{
    Monster owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.45f)
        {
            owner.Agent.speed = 0;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);  
    }
}
