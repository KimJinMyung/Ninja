using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Battle : MonoBehaviour
{
    private Player owner;

    private GameObject AttackCollider;

    private LayerMask AssassinatedLayer;

    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashParry = Animator.StringToHash("Parry");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashAssasinated = Animator.StringToHash("Assasinated");
    protected readonly int hashForward = Animator.StringToHash("Forward");
    protected readonly int hashAttackAble = Animator.StringToHash("IsAttackAble");
    protected readonly int hashBattleMode = Animator.StringToHash("BattleMode");
    protected readonly int hashBattleModeChanged = Animator.StringToHash("BattleModeChanged");

    private void Awake()
    {
        owner = GetComponent<Player>();
        AttackBox attackBox = GetComponentInChildren<AttackBox>();
        AttackCollider = attackBox.GetComponent<Collider>().gameObject;

        AssassinatedLayer = LayerMask.GetMask("Monster", "LockOnAble", "LockOnTarget", "Incapacitated");
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, transform.forward * 1.5f);
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if (owner.ViewModel == null) return;
        if (owner.ViewModel.playerState == State.Parry) return;
        if (owner.ViewModel.playerState == State.Assasinate) return;

        if (context.started)
        {
            if(!owner.Animator.GetBool(hashAttackAble)) return;
            if (owner.Animator.GetBool(hashDefence))
            {
                owner.Animator.SetTrigger(hashParry);
                return;
            }

            if(!owner.Animator.GetBool(hashBattleMode))
            {
                owner.Animator.SetTrigger(hashBattleModeChanged);
                return;
            }

            if (Physics.Raycast(owner.transform.position + Vector3.up, owner.transform.forward, out RaycastHit hit, 2f))
            {
                Debug.Log(hit.transform.name);

                Monster monster = hit.transform.GetComponent<Monster>();

                if(monster != null)
                {
                    float dotProductWithPlayer = Vector3.Dot(monster.transform.forward, owner.transform.forward);
                    //플레이어가 몬스터와 마주보고 있다.
                    if (dotProductWithPlayer < 0.5f)
                    {
                        if (monster != null && monster.MonsterViewModel.MonsterState == State.Incapacitated)
                        {
                            owner.transform.position = monster.transform.position + monster.transform.forward * 3f;
                            //전방에서 몬스터를 즉사시키는 모션 실행
                            owner.Animator.SetBool(hashForward, true);
                            monster.animator.SetBool(hashForward, true);

                            owner.Animator.SetTrigger(hashAssasinated);
                            monster.animator.SetTrigger(hashAssasinated);
                            owner.ViewModel.RequestStateChanged(owner.player_id, State.Assasinate);
                            return;
                        }
                    }
                    //플레이어가 몬스터의 등을 바라보고 있다.
                    else if (dotProductWithPlayer > 0.3f)
                    {
                        if (monster.MonsterViewModel.TraceTarget == null || monster.MonsterViewModel.MonsterState == State.Incapacitated)
                        {
                            owner.transform.position = monster.transform.position - monster.transform.forward * 0.8f;
                            //등 뒤에서 몬스터를 즉사시키는 모션 실행
                            owner.Animator.SetBool(hashForward, false);
                            monster.animator.SetBool(hashForward, false);

                            owner.Animator.SetTrigger(hashAssasinated);
                            monster.animator.SetTrigger(hashAssasinated);
                            owner.ViewModel.RequestStateChanged(owner.player_id, State.Assasinate);
                            return;
                        }
                    }
                }                
            }

            owner.Animator.SetBool(hashAttackAble, false);
            owner.Animator.SetTrigger(hashAttack);
        }
    }

    public void OnDefence(InputAction.CallbackContext context)
    {
        if (owner.Player_Info.Stamina < 10) return;

        owner.isDefence = context.ReadValue<float>() > 0.5f;

        if (!owner.Animator.GetBool(hashBattleMode))
        {
            owner.Animator.SetTrigger(hashBattleModeChanged);
        }

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
