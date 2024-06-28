using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Dead : StateMachineBehaviour
{
    private Monster owner;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Monster>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f)
        {
            owner.Dead();
        }
    }
}
