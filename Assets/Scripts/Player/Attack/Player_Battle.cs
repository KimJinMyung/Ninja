using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Battle : MonoBehaviour
{
    [SerializeField]
    private Transform AttackColliderPos;

    [SerializeField] private Transform DefenceColliderPos;

    private Player owner;

    private HashSet<Collider> hitMonsters = new HashSet<Collider>();

    private void Awake()
    {
        owner = GetComponent<Player>();
    }

    private void OnEnable()
    {
        owner.Animator.SetBool("IsAttackAble", true);
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
                hitMonsters.Clear();
                owner.Animator.SetTrigger("Attack");
            }
        }
    }

    public void OnDefence(InputAction.CallbackContext context)
    {
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.matrix = Matrix4x4.TRS(AttackColliderPos.position, AttackColliderPos.rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(0.05f, 0.9f, 0.05f));
    }

    private void Update()
    {
        if (owner.ViewModel.playerState != State.Attack) return;

        Attacking();

        Debug.Log(owner.ViewModel.playerState);
    }

    public void Attacking()
    {
        if (owner.ViewModel == null) return;

        Collider[] hitColliders = Physics.OverlapBox(AttackColliderPos.position, new Vector3(0.05f, 0.9f, 0.05f), AttackColliderPos.rotation, LayerMask.NameToLayer("Monster"));
        foreach (Collider collider in hitColliders)
        {
            if (collider.transform == this.transform) continue;
            if (!hitMonsters.Contains(collider))
            {
                //데미지 부여 로직 추가
                Debug.Log($"{collider.name} 데미지 부여");

                //데미지 부여한 몬스터 목록 추가
                hitMonsters.Add(collider);
            }                
        }
    }

    public void AttackStart()
    {
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Attack);
    }

    public void AttackEnd()
    {
        owner.Animator.SetBool("IsAttackAble", true);
        owner.ViewModel.RequestStateChanged(owner.player_id, State.Battle);
    }
}
