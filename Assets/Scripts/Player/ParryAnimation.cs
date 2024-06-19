using Player_State.Extension;
using UnityEngine;

public class ParryAnimation : StateMachineBehaviour
{
    private Player player;

    protected readonly int hashParry = Animator.StringToHash("Parry");
    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashMoveAble = Animator.StringToHash("IsMoveAble");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashMoveAble, false);

        if (player == null) { player = animator.transform.root.GetComponent<Player>(); }
        player.InputVm.RequestStateChanged(player.PlayerId, State.Parry);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {       
        if(player.InputVm.PlayerState != State.Battle)
        {
            if (animator.GetBool(hashDefence))
            {
                player.InputVm.RequestStateChanged(player.PlayerId, State.Defence);
            }
            else
            {
                player.InputVm.RequestStateChanged(player.PlayerId, State.Battle);
            }
        }

        animator.SetBool(hashMoveAble, true);
        animator.ResetTrigger(hashParry);
    }
}
