using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.iOS;

public class Monster_Dead : StateMachineBehaviour
{
    private Monster owner;
    private Monster_data data;

    protected readonly int hashHurt = Animator.StringToHash("Hurt");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Monster>();
        owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Die);
        animator.ResetTrigger(hashHurt);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.MonsterViewModel.MonsterInfo.Life > 0) return;

        if(stateInfo.normalizedTime >= 1f)
        {
            owner.gameObject.SetActive(false);
        }
    }
}
