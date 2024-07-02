using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_HangUp : StateMachineBehaviour
{
    private Player owner;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.95f)
        {
            owner.playerController.enabled = true;

            owner.ViewModel.RequestStateChanged(owner.player_id, animator.GetBool("BattleMode")? State.Battle : State.Idle);
            animator.SetBool("Climbing", false);
            return;
        }

        owner.Animator.MatchTarget(owner.currentAction.MatchPos, owner.transform.rotation,
           owner.currentAction.MatchBodyPart,
           new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), owner.currentAction.MatchStartTime, owner.currentAction.MatchTargetTime);
        //if (owner.currentAction.EnableTargetMatching)
        //{
        //    MatchTarget(owner.currentAction);
        //}
    }
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.SetBool("IsMoveAble", true);
    }

    //void MatchTarget(ParkourAction action)
    //{
    //    if (owner.Animator.isMatchingTarget) return;

    //    owner.Animator.MatchTarget(action.MatchPos, owner.transform.rotation,
    //        action.MatchBodyPart,
    //        new MatchTargetWeightMask(new Vector3(0, 1, 0), 0), action.MatchStartTime, action.MatchTargetTime);
    //}
}
