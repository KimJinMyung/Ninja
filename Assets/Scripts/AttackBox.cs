using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackBox : MonoBehaviour
{
     private LayerMask _attackLayer;

    //���� �޴� Collider
    List<Transform> hitCollider;

    private List<Transform> _attackedMonster = new List<Transform>();
    public List<Transform> AttackedMonster {  get { return _attackedMonster; } }

    private Player owner;
    private Collider attackCollider;

    private void Awake()
    {
        owner = transform.root.GetComponentInChildren<Player>();
        attackCollider = GetComponent<Collider>();
        _attackLayer = LayerMask.GetMask("Monster", "LockOnAble", "LockOnTarget");
    }

    private void OnEnable()
    {
        hitCollider = new List<Transform>();
        owner.Animator.SetBool("IsAttackAble", true);
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
        if (owner.ViewModel.playerState != State.Attack) return;

        Attacking();
        Debug.Log(owner.ViewModel.playerState);
    }

    public void Attacking()
    {
        if (owner.ViewModel == null) return;

        foreach (var collider in hitCollider)
        {
            if (collider.transform == this.transform) continue;
            if (!_attackedMonster.Contains(collider))
            {
                //������ �ο� ���� �߰�
                Debug.Log($"{collider.name} ������ �ο�");

                //������ �ο��� ���� ��� �߰�
                _attackedMonster.Add(collider);
            }
        }
    }
}
