using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleModeChangedEnd : StateMachineBehaviour
{
    private Player owner;
    private bool IsBattleMode;
    protected readonly int hashBattleMode = Animator.StringToHash("BattleMode");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Player>();

        IsBattleMode = animator.GetBool(hashBattleMode) ? false : true;

        animator.SetBool(hashBattleMode, IsBattleMode);
        animator.SetLayerWeight(layerIndex, 1);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime > 0.1f)
        {
            owner.Katana.SetActive(IsBattleMode);
            owner.KatanaCover.SetActive(!IsBattleMode);
        }

        if(stateInfo.normalizedTime > 0.4f)
        {
            animator.SetLayerWeight(layerIndex, 1 - stateInfo.normalizedTime % 1);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetLayerWeight(layerIndex, 0);
    }
}
