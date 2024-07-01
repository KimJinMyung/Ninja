using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAssassinatedEnd : StateMachineBehaviour
{
    private Player owner;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
    }
}
