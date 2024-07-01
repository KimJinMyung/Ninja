using UnityEngine;

public class Player_ClimbEnd : StateMachineBehaviour
{
    private Player owner;
    private State player_state;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();
        player_state = owner.ViewModel.playerState;
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Climbing);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner.ViewModel.RequestStateChanged(owner.player_id, player_state);
    }
}
