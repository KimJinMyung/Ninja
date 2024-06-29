using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Battle : MonoBehaviour
{
    private Player owner;

    private GameObject AttackCollider;

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
            if(!owner.Animator.GetBool("IsAttackAble")) return;
            //if (owner.ViewModel.playerState == State.Attack) return;

            if (owner.ViewModel.playerState == State.Defence) owner.ViewModel.RequestStateChanged(owner.player_id, State.Parry);
            else
            {
                if (owner.ViewModel.playerState == State.Parry) return;
                owner.Animator.SetBool("IsAttackAble", false);
                owner.Animator.SetTrigger("Attack");
            }
        }
    }

    public void OnDefence(InputAction.CallbackContext context)
    {
        if (owner.Player_Info.Stamina < 10) return;

        bool isDefence = context.ReadValue<float>() > 0.5f;

        if (isDefence)
        {
            owner.Animator.SetBool("Defence", true);
        }
        else
        {
            owner.Animator.SetBool("Defence", false);
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
        owner.Animator.SetBool("IsAttackAble", true);
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
    }
}
