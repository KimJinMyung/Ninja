using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Profiling.HierarchyFrameDataView;

public class Player_Climbing : StateMachineBehaviour
{
    private Player owner;
    private State currentPlayerState;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();
        currentPlayerState = owner.ViewModel.playerState;
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Climbing);

        startPosition = owner.transform.position;
        targetPosition = owner.ClimbingPos;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.9f)
        {
            owner.ViewModel.RequestStateChanged(owner.player_id, currentPlayerState);
            animator.SetBool("Climbing", false);
            return;
        }

        float climbProcess = stateInfo.normalizedTime;
        Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, climbProcess);
        owner.playerController.Move(newPos - owner.transform.position);        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.playerController.enabled = false;
        owner.transform.position = targetPosition;
        owner.playerController.enabled = true;
    }
}
