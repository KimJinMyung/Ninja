using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Assainated : StateMachineBehaviour
{
    private Monster owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Die);
    }
}
