using UnityEngine;
using UnityEngine.InputSystem;

public enum AssassinatedType
{
    Forward,
    Backward,
    UpForward,
    UpBackward
}

public class Player_Battle : MonoBehaviour
{
    private Player owner;
    private Player_LockOn ownerViewZone;

    private GameObject AttackCollider;

    private int AssassinatedLayer;

    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashParry = Animator.StringToHash("Parry");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashAssasinated = Animator.StringToHash("Assasinated");
    protected readonly int hashForward = Animator.StringToHash("Forward");
    protected readonly int hashAttackAble = Animator.StringToHash("IsAttackAble");
    protected readonly int hashBattleMode = Animator.StringToHash("BattleMode");
    protected readonly int hashBattleModeChanged = Animator.StringToHash("BattleModeChanged");

    public bool IsUpperPlayerToMonster;
    private Monster ViewMonster;

    private void Awake()
    {
        owner = GetComponent<Player>();
        ownerViewZone = GetComponent<Player_LockOn>();
        AttackBox attackBox = GetComponentInChildren<AttackBox>();
        AttackCollider = attackBox.GetComponent<Collider>().gameObject;

        AssassinatedLayer = LayerMask.GetMask("Monster", "LockOnAble", "LockOnTarget", "Incapacitated");

        owner.Katana.SetActive(false);
    }

    private void Update()
    {
        if (ownerViewZone.ViewModel.LockOnAbleTarget == null) return;
        ViewMonster = ownerViewZone.ViewModel.LockOnAbleTarget.GetComponent<Monster>();

        if (ViewMonster == null) return;
        if ((owner.transform.position.y - (ViewMonster.transform.position.y + ViewMonster.monsterHeight)) > 0f) IsUpperPlayerToMonster = true;
        else IsUpperPlayerToMonster = false;
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
                owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
                return;
            }
            
            if (!IsUpperPlayerToMonster)
            {
                if (Physics.Raycast(owner.transform.position + Vector3.up, owner.transform.forward, out RaycastHit hit, 2f, AssassinatedLayer))
                {                    
                    Monster monster = hit.transform.GetComponent<Monster>();
                    
                    if (monster != null)
                    {
                        float dotProductWithPlayer = Vector3.Dot(monster.transform.forward, owner.transform.forward);

                        //플레이어가 몬스터와 마주보고 있다.
                        if (dotProductWithPlayer < 0.5f)
                        {
                            if (monster != null && monster.MonsterViewModel.MonsterState == State.Incapacitated)
                            {
                                owner.ViewModel.RequestAssassinatedType(AssassinatedType.Forward, monster);
                                owner.ViewModel.RequestStateChanged(owner.player_id, State.Assasinate);
                                return;
                            }
                        }
                        //플레이어가 몬스터의 등을 바라보고 있다.
                        else if (dotProductWithPlayer > 0.3f)
                        {
                            if (monster.MonsterViewModel.TraceTarget == null || monster.MonsterViewModel.MonsterState == State.Incapacitated)
                            {
                                owner.ViewModel.RequestAssassinatedType(AssassinatedType.Backward, monster);
                                owner.ViewModel.RequestStateChanged(owner.player_id, State.Assasinate);
                                return;
                            }
                        }
                    }
                }
            }
            else if(ViewMonster != null && ViewMonster.Type != MonsterType.Boss)
            {
                float dotProductWithPlayer = Vector3.Dot(ViewMonster.transform.forward, owner.transform.forward);

                //플레이어가 몬스터와 마주보고 있다.
                if (dotProductWithPlayer < 0.5f)
                {
                    owner.ViewModel.RequestAssassinatedType(AssassinatedType.UpForward, ViewMonster);
                    owner.ViewModel.RequestStateChanged(owner.player_id, State.Assasinate);
                    return;
                }
                //플레이어가 몬스터의 등을 바라보고 있다.
                else 
                {
                    owner.ViewModel.RequestAssassinatedType(AssassinatedType.UpBackward, ViewMonster);
                    owner.ViewModel.RequestStateChanged(owner.player_id, State.Assasinate);
                    return;
                }
            }
           

            owner.Animator.SetBool(hashAttackAble, false);
            owner.Animator.SetTrigger(hashAttack);
            //owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
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
