using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryEnd : StateMachineBehaviour
{
    [SerializeField] string _triggerName;

    protected readonly int hashDefence = Animator.StringToHash("Defence");

    private Player owner;
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();
        animator.SetBool(hashDefence, false);
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Parry);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime >= 0.3f)
        {
            owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(owner.isDefence) animator.SetBool(hashDefence, true);
        animator.ResetTrigger(_triggerName);
    }
}
