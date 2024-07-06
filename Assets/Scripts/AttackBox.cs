using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackBox : MonoBehaviour
{
     private LayerMask _attackLayer;

    //공격 받는 Collider
    List<Transform> hitCollider;

    private List<Transform> _attackedMonster = new List<Transform>();
    public List<Transform> AttackedMonster {  get { return _attackedMonster; } }

    private Player owner_player;
    private Monster owner_monster;

    private void Awake()
    {
        owner_player = transform.GetComponentInParent<Player>();
        owner_monster = transform.GetComponentInParent<Monster>();

        if (owner_player != null)
        {
            _attackLayer = LayerMask.GetMask("Monster", "LockOnAble", "LockOnTarget", "Incapacitated");
        }
        else if(owner_monster != null)
        {
            _attackLayer = LayerMask.GetMask("Player");
        }
    }

    private void OnEnable()
    {
        hitCollider = new List<Transform>();

        if(owner_player != null)
        {
            owner_player.Animator.SetBool("IsAttackAble", true);
        }

        hitCollider.Clear();
        _attackedMonster.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(((1 << other.gameObject.layer) & _attackLayer) != 0)
        {
            hitCollider.Add(other.transform);
        }
    }

    private void Update()
    {
        if (owner_player != null)
        {
            if (owner_player.ViewModel.playerState != State.Attack) return;
        }
        else if (owner_monster != null)
        {
            if (owner_monster.MonsterViewModel.MonsterState != State.Attack) return;
        }

        Attacking();
    }

    public void Attacking()
    {
        foreach (var collider in hitCollider)
        {
            if (!this.gameObject.activeSelf) return;

            if (collider.transform == this.transform) continue;
            if (!_attackedMonster.Contains(collider))
            {
                if(owner_player != null)
                {
                    Monster target = collider.GetComponent<Monster>();
                    if(target != null)
                    {
                        target.Hurt(owner_player.Player_Info.ATK, owner_player);
                    }                    
                }else if(owner_monster != null)
                {
                    Player target = collider.GetComponent<Player>();
                    if(target != null)
                    {
                        target.Hurt(owner_monster, owner_monster.MonsterViewModel.MonsterInfo.ATK);
                    }
                }

                //데미지 부여한 몬스터 목록 추가
                _attackedMonster.Add(collider);
            }
        }
    }
}
