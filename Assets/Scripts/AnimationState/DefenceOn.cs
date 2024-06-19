using UnityEngine;

public class DefenceOn : StateMachineBehaviour
{
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashIsMoveAble, false);
    }
}
