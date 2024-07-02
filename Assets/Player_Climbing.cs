using UnityEngine;

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
        animator.SetBool("IsMoveAble", false);
        owner.playerController.enabled = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= 0.95f)
        {
            owner.playerController.enabled = true;

            owner.ViewModel.RequestStateChanged(owner.player_id, currentPlayerState);
            animator.SetBool("Climbing", false);
            return;
        }

        if (owner.currentAction.EnableTargetMatching)
        {
            MatchTarget(owner.currentAction);
        }

        //float climbProcess = stateInfo.normalizedTime;
        //Vector3 newPos = Vector3.Lerp(startPosition, targetPosition, climbProcess);
        //owner.playerController.Move(newPos - owner.transform.position);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        animator.SetBool("IsMoveAble", true);
    }

    void MatchTarget(ParkourAction action)
    {
        if (owner.Animator.isMatchingTarget) return;

        owner.Animator.MatchTarget(action.MatchPos, owner.transform.rotation, 
            action.MatchBodyPart, 
            new MatchTargetWeightMask(new Vector3(0,1,0), 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
