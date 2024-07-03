using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEngine.UI.GridLayoutGroup;

public class AttackReset : StateMachineBehaviour
{
    [SerializeField] string _triggerName;

    private int _attackCount = -1;

    protected readonly int hashIsAttackCount = Animator.StringToHash("AttackCount");
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    protected readonly int hashIsAttackAble = Animator.StringToHash("IsAttackAble");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _attackCount = animator.GetInteger(hashIsAttackCount);
        _attackCount = (_attackCount + 1) % 4;
        animator.applyRootMotion = true;
        animator.SetInteger(hashIsAttackCount, _attackCount);

        animator.SetBool(hashIsMoveAble, true);
    }
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime > 0.2f)
            animator.SetBool(hashIsMoveAble, false);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool(hashIsMoveAble, true);
        animator.SetBool(hashIsAttackAble, true);
        animator.ResetTrigger(_triggerName);
    }

    public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
    {
        animator.SetInteger(hashIsAttackCount, -1);
        animator.SetBool(hashIsMoveAble, true);
        animator.applyRootMotion = false;
    }
}
