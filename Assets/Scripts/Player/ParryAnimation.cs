using UnityEngine;

public class ParryAnimation : StateMachineBehaviour
{
    private Player player;

    protected readonly int hashParry = Animator.StringToHash("Parry");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (player == null) { player = animator.transform.root.GetComponent<Player>(); }
        player.InputVm.PlayerState = State.Parry;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        if(player.InputVm.PlayerState != State.Battle)
            player.InputVm.PlayerState = State.Defence;

        animator.ResetTrigger(hashParry);
    }
}
