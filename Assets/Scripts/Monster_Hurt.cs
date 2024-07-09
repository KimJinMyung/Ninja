using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Hurt : StateMachineBehaviour
{
    Monster owner;

    protected readonly int hashAttack = Animator.StringToHash("Attack");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(hashAttack);

        if(owner.MonsterViewModel.MonsterInfo.HP > 0) owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);
    }
}
