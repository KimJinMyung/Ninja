using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAttack : StateMachineBehaviour
{
    private Monster owner;
    private Monster_Attack attack;

    [Header("Throw Weapons")]
    [SerializeField] GameObject shuriken;

    private bool isAction;

    protected readonly int hashAttackIndex = Animator.StringToHash("AttackIndex");
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.transform.GetComponent<Monster>();
        if(animator.layerCount >= 2)
            animator.SetLayerWeight(1, 0);

        attack = owner.MonsterViewModel.CurrentAttackMethod;
        isAction = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(attack.DataName == Enum.GetName(typeof(WeaponsType), WeaponsType.ShurikenAttack) && !isAction)
        {
            if((animator.GetInteger(hashAttackIndex) == 0 && stateInfo.normalizedTime >= 0.34f) || (animator.GetInteger(hashAttackIndex) == 1 && stateInfo.normalizedTime >= 0.297f))
            {
                isAction = true;
                Debug.Log("¼ö¸®°Ë ´øÁü");
                Quaternion CreateDir = Quaternion.LookRotation(owner.MonsterViewModel.TraceTarget.position + Vector3.up - owner.ThrowShurikenPoint.position);
                Shuriken shootShuriken = Instantiate(shuriken, owner.ThrowShurikenPoint.position, CreateDir).GetComponent<Shuriken>();
                shootShuriken.SetShooterData(owner);
            }
        }        
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (animator.layerCount >= 2)
            animator.SetLayerWeight(1, 1);

        if(attack.AttackType == "Long")
        { 
             owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Battle);
        }
        else
        {
            if (owner.MonsterViewModel.MonsterInfo.Stamina > 0)
                owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.RetreatAfterAttack);
        }
        
    }
}
