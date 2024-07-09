using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Dead : StateMachineBehaviour
{
    private Player owner;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Player>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 1f && owner.ViewModel.playerInfo.Life > 0)
        {
            owner.isResurrectionAble = true;
        }
    }
}
