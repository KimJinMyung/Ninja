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

    private bool isJump;
    private bool isStop;
    Vector3 jumpDirection;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        owner = animator.GetComponent<Monster>();
        target = owner.MonsterViewModel.TraceTarget;
        isStop = false;
        isJump = false;

        jumpDirection = (target.position - owner.transform.position);
        jumpDirection.y = 0;
        jumpDirection.Normalize();

        switch (owner.BossAttackTypeIndex)
        {
            case 0:
                if (isNotEnd)
                {                    
                    if(owner.BossCurrentAttackIndex == 0)
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
                animator.applyRootMotion = false;
                owner.rb.isKinematic = false;
                owner.Agent.enabled = false;
                owner.Agent.stoppingDistance = default;
                owner.rb.AddForce(jumpDirection * DashPower, ForceMode.Impulse);
                break;
        }
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        switch(owner.BossAttackTypeIndex)
        {
            case 0:
                if (isNotEnd)
                {
                    if (owner.BossCurrentAttackIndex == 1 && stateInfo.normalizedTime >= 0.5f && !isJump)
                    {
                        isJump = true;
                        owner.rb.AddForce(Vector3.up * JumpPower + JumpDashPower * jumpDirection, ForceMode.Impulse);
                        return;
                    }
                    
                    if (stateInfo.normalizedTime >= 1f && !isStop)
                    {
                        owner.StartCoroutine(StartNextAttack());
                        return;
                    }
                }
                else
                {
                    if(stateInfo.normalizedTime <= 0.35f)
                    {
                        Vector3 dir = target.position - owner.transform.position;
                        dir.y = 0;
                        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
                        owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRotation, 1.5f * Time.deltaTime);
                    }
                }
                
                
                break;
            case 1:

                break;
            case 2:
                if (stateInfo.normalizedTime >= 0.3f)
                {
                    owner.rb.velocity = Vector3.Lerp(owner.rb.velocity, new Vector3(0, owner.rb.velocity.y, 0), 50f * Time.deltaTime);
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
