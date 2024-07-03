using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_GrapplingStart : StateMachineBehaviour
{
    private RopeAction owner;

    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<RopeAction>();
        animator.applyRootMotion = false;
        animator.SetBool(hashIsMoveAble, false);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f)
        {
            owner.ExecuteGrapple();
        }
    }

    //public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
        
    //}
}
