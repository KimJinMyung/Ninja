using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenceEnd : StateMachineBehaviour
{
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashIsMoveAble, true);
    }
}
