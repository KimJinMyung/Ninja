using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class BossMonster_AttackEnd : StateMachineBehaviour
{
    Monster owner;
    Transform target;

    [Header("Jump Attack")]
    [SerializeField] private float JumpPower = 7f;
    [SerializeField] private float JumpDashPower = 8f;
    [SerializeField] private float gravityMultiplier = 0.5f;

    [Header("Dash Attack")]
    [SerializeField] private float DashPower = 15f;

    [Header("End Animation?")]
    [SerializeField] private bool isNotEnd;

    private bool isStop;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
        target = owner.MonsterViewModel.TraceTarget;
        isStop = false;

        switch (owner.BossAttackTypeIndex)
        {
            case 0:
                Vector3 jumpDirection = (target.position - owner.transform.position);
                jumpDirection.y = 0;
                jumpDirection.Normalize();
                if (isNotEnd)
                {                    
                    owner.rb.AddForce(Vector3.up * JumpPower + JumpDashPower * jumpDirection, ForceMode.Impulse);
                }
                else
                {
                    owner.rb.useGravity = true;
                    owner.Agent.enabled = false;
                    owner.rb.isKinematic = false;
                    owner.rb.AddForce(Vector3.down * JumpPower + JumpDashPower * jumpDirection, ForceMode.Impulse);
                }
                break;
            case 2:                
                owner.rb.AddForce(owner.transform.forward * DashPower, ForceMode.Impulse);
                break;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch(owner.BossAttackTypeIndex)
        {
            case 0:
                if(stateInfo.normalizedTime >= 1f && !isStop)
                {
                    owner.StartCoroutine(StartNextAttack());
                    return;
                }
                
                break;
            case 2:
                if (stateInfo.normalizedTime >= 0.3f)
                {
                    owner.rb.velocity = Vector3.Lerp(owner.rb.velocity, new Vector3(0, owner.rb.velocity.y,0), 50f * Time.deltaTime);
                }

                if (stateInfo.normalizedTime >= 0.45f)
                {
                    owner.rb.velocity = Vector3.up * owner.rb.velocity.y;
                }
                break;
        }
    }

    IEnumerator StartNextAttack()
    {
        isStop = true;
        owner.rb.useGravity = false;
        owner.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        owner.animator.SetTrigger("NextAttack");
        owner.rb.useGravity = true;
        owner.rb.isKinematic = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!isNotEnd)
        {
            owner.rb.isKinematic = true;
            owner.Agent.enabled = true;
            owner.Agent.ResetPath();

            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);
        }        
    }
}
