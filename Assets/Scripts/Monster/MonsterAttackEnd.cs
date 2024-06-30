using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackEnd : StateMachineBehaviour
{
    private Monster owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Monster>();
        animator.SetLayerWeight(1, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 1);
        if(owner.MonsterViewModel.MonsterState != State.Parried)
             owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
    }
}
