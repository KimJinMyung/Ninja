using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Battle : MonoBehaviour
{
    private Player owner;

    private GameObject AttackCollider;

    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashParry = Animator.StringToHash("Parry");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashAttackAble = Animator.StringToHash("IsAttackAble");

    private void Awake()
    {
        owner = GetComponent<Player>();
        AttackBox attackBox = GetComponentInChildren<AttackBox>();
        AttackCollider = attackBox.GetComponent<Collider>().gameObject;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (owner.ViewModel == null) return;

        if (context.started)
        {
            if(!owner.Animator.GetBool(hashAttackAble)) return;
            //if (owner.ViewModel.playerState == State.Attack) return;

            if (owner.Animator.GetBool(hashDefence)) owner.Animator.SetTrigger(hashParry);/*owner.ViewModel.RequestStateChanged(owner.player_id, State.Parry);*/
            else
            {
                if (owner.ViewModel.playerState == State.Parry) return;
                owner.Animator.SetBool(hashAttackAble, false);
                owner.Animator.SetTrigger(hashAttack);
            }
        }
    }

    public void OnDefence(InputAction.CallbackContext context)
    {
        if (owner.Player_Info.Stamina < 10) return;

        owner.isDefence = context.ReadValue<float>() > 0.5f;

        if (owner.isDefence)
        {
            owner.Animator.SetBool(hashDefence, true);
        }
        else
        {
            owner.Animator.SetBool(hashDefence, false);
        }
    }

    public void AttackStart()
    {
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Attack);
        AttackCollider.SetActive(true);
    }

    public void AttackEnd()
    {
        AttackCollider.SetActive(false);
        owner.Animator.SetBool(hashAttackAble, true);
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
    }
}
