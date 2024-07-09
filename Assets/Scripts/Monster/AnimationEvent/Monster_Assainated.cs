using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_Assainated : StateMachineBehaviour
{
    private Monster owner;
    private Monster_data data;

    protected readonly int hashDead = Animator.StringToHash("Dead");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
        animator.SetLayerWeight(1, 0);
        owner.MonsterViewModel.MonsterInfo.HP = 0;
        animator.SetBool(hashDead, true);
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.MonsterViewModel.MonsterInfo.Life > 0) return;

        if (stateInfo.normalizedTime >= 1f)
        {
            owner.gameObject.SetActive(false);
            MonsterManager.instance.DieMonster(owner);
        }
    }
}
