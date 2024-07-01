using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Assainated : StateMachineBehaviour
{
    private Monster owner;

    protected readonly int hashDead = Animator.StringToHash("Dead");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
        owner.MonsterViewModel.MonsterInfo.HP = 0;
        animator.SetBool(hashDead, true);
        animator.SetLayerWeight(1, 0);

        owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Die);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f)
        {
            owner.gameObject.SetActive(false);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(1, 1);
    }
}
