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

    private void Awake()
    {
        owner_player = transform.root.GetComponentInChildren<Player>();

        if(owner_player != null)
        {
            _attackLayer = LayerMask.GetMask("Monster", "LockOnAble", "LockOnTarget");
        }
    }

    private void OnEnable()
    {
        hitCollider = new List<Transform>();
        if(owner_player != null)
        {
            owner_player.Animator.SetBool("IsAttackAble", true);
        }
    }

    private void OnDisable()
    {
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

    //private void OnTriggerExit(Collider other)
    //{
    //    if (hitCollider.Contains(other.transform))
    //    {
    //        hitCollider.Remove(other.transform);
    //    }
    //}

    private void Update()
    {
        if (owner_player.ViewModel.playerState != State.Attack) return;

        Attacking();
        Debug.Log(owner_player.ViewModel.playerState);
    }

    public void Attacking()
    {
        if(owner_player != null)
        {
            if (owner_player.ViewModel == null) return;
        }

        foreach (var collider in hitCollider)
        {
            if (collider.transform == this.transform) continue;
            if (!_attackedMonster.Contains(collider))
            {
                //데미지 부여 로직 추가
                Debug.Log($"{collider.name} 데미지 부여");
                if(owner_player != null)
                {
                    Monster target = collider.GetComponent<Monster>();
                    if(target != null)
                    {
                        target.Hurt(owner_player.Player_Info.ATK, owner_player);
                    }                    
                }

                //데미지 부여한 몬스터 목록 추가
                _attackedMonster.Add(collider);
            }
        }
    }
}
