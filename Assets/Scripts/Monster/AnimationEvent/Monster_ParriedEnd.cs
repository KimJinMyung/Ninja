using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_ParriedEnd : StateMachineBehaviour
{
    private Monster owner;
    private int monsterId;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Monster>();
        monsterId = owner.monsterId;
        if (owner.MonsterViewModel.MonsterInfo.Stamina <= 0)
        {
            animator.SetBool("Parry", true);
            owner.MonsterViewModel.RequestStateChanged(monsterId, State.Incapacitated);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.MonsterViewModel.MonsterInfo.Stamina > 0) owner.MonsterViewModel.RequestStateChanged(monsterId, State.Battle);
    }
}
