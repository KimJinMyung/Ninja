using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HurtEnd : StateMachineBehaviour
{
    private Player owner;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetBool("BattleMode"))
        {
            owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
        }
        else
        {
            owner.ViewModel.RequestStateChanged(owner.player_id, State.Idle);
        }
    }
}
