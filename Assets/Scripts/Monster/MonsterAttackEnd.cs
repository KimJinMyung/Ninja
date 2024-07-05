using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttackEnd : StateMachineBehaviour
{
    private Monster owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Monster>();
        if(animator.layerCount >= 2)
            animator.SetLayerWeight(1, 0);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.layerCount >= 2)
            animator.SetLayerWeight(1, 1);

        if(owner.MonsterViewModel.CurrentAttackMethod.AttackType == "Long")
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Battle);
        }
        else
        {
            if (owner.MonsterViewModel.MonsterInfo.Stamina > 0)
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
        }
        
    }
}
