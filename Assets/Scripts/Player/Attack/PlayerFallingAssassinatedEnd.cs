using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallingAssassinatedEnd : StateMachineBehaviour
{
    private Player owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Player>();
        owner.playerController.enabled = true;
        animator.applyRootMotion = true;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
        animator.SetBool("IsMoveAble", true);
        animator.applyRootMotion =false;
    }
}
