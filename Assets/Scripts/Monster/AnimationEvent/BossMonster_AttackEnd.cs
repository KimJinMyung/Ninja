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

    protected static int hashAttackMove = Animator.StringToHash("AttackMove");

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
                    owner.rb.useGravity = false;
                    owner.Agent.enabled = false;
                    owner.rb.isKinematic = false;

                    if (owner.BossCurrentAttackIndex == 0)
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
            case 1:
                animator.applyRootMotion = false;
                owner.rb.isKinematic = true;
                owner.Agent.enabled = true;
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
        if(owner.MonsterViewModel.MonsterInfo.Stamina <= 0)
        {
            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Incapacitated);
            return;
        }

        switch (owner.BossAttackTypeIndex)
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
                    if (stateInfo.normalizedTime <= 0.35f)
                    {
                        Vector3 dir = target.position - owner.transform.position;
                        dir.y = 0;
                        Quaternion targetRotation = Quaternion.LookRotation(dir.normalized);
                        owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, targetRotation, 1.5f * Time.deltaTime);
                    }

                    if (stateInfo.normalizedTime >= 0.21f) owner.attackBox.gameObject.SetActive(false);
                }

                break;
            case 1:

                switch (owner.BossCurrentAttackIndex)
                {
                    case 0:
                        if ((stateInfo.normalizedTime >= 0.11f && stateInfo.normalizedTime <= 0.175f) || (stateInfo.normalizedTime >= 0.29f && stateInfo.normalizedTime <= 0.37f) || 
                            (stateInfo.normalizedTime >= 0.51f && stateInfo.normalizedTime <= 0.66f)) animator.SetBool(hashAttackMove, true);
                        else animator.SetBool(hashAttackMove, false);

                        if ((stateInfo.normalizedTime >= 0.05f && stateInfo.normalizedTime <= 0.13f) || (stateInfo.normalizedTime >= 0.2f && stateInfo.normalizedTime <= 0.37f) || 
                            (stateInfo.normalizedTime >= 0.51f && stateInfo.normalizedTime <= 0.66f)) owner.attackBox.gameObject.SetActive(true);
                        else owner.attackBox.gameObject.SetActive(false);

                        break;
                    case 1:
                        if ((stateInfo.normalizedTime >= 0.06f && stateInfo.normalizedTime <= 0.1f) || (stateInfo.normalizedTime >= 0.2f && stateInfo.normalizedTime <= 0.45f) || 
                            (stateInfo.normalizedTime >= 0.6f && stateInfo.normalizedTime <= 0.64f)) animator.SetBool(hashAttackMove, true);
                        else animator.SetBool(hashAttackMove, false);

                        if ((stateInfo.normalizedTime >= 0f && stateInfo.normalizedTime <= 0.12f) || (stateInfo.normalizedTime >= 0.2f && stateInfo.normalizedTime <= 0.32f) || 
                            (stateInfo.normalizedTime >= 0.33f && stateInfo.normalizedTime <= 0.39f) || (stateInfo.normalizedTime >= 0.4f && stateInfo.normalizedTime <= 0.51f) || 
                            (stateInfo.normalizedTime >= 0.58f && stateInfo.normalizedTime <= 0.68f)) owner.attackBox.gameObject.SetActive(true);
                        else owner.attackBox.gameObject.SetActive(false);

                        break;
                    case 2:
                        if ((stateInfo.normalizedTime >= 0.03f && stateInfo.normalizedTime <= 0.1f) || (stateInfo.normalizedTime >= 0.18f && stateInfo.normalizedTime <= 0.27f) || 
                            (stateInfo.normalizedTime >= 0.4f && stateInfo.normalizedTime <= 0.43f) || (stateInfo.normalizedTime >= 0.63f && stateInfo.normalizedTime <= 0.65f)) animator.SetBool(hashAttackMove, true);
                        else owner.attackBox.gameObject.SetActive(false);

                        if ((stateInfo.normalizedTime >= 0.07f && stateInfo.normalizedTime <= 0.15f) || (stateInfo.normalizedTime >= 0.18f && stateInfo.normalizedTime <= 0.23f) ||
                            (stateInfo.normalizedTime >= 0.25f && stateInfo.normalizedTime <= 0.31f) || (stateInfo.normalizedTime >= 0.35f && stateInfo.normalizedTime <= 0.44f) ||
                            (stateInfo.normalizedTime >= 0.53f && stateInfo.normalizedTime <= 0.65f) || (stateInfo.normalizedTime >= 0.66f && stateInfo.normalizedTime <= 0.76f)) owner.attackBox.gameObject.SetActive(true);
                        else owner.attackBox.gameObject.SetActive(false);

                        break;
                }


                Vector3 vecToTarget = target.position - owner.transform.position;
                vecToTarget.y = 0;
                vecToTarget.Normalize();
                Quaternion rotation = Quaternion.LookRotation(vecToTarget);

                owner.Agent.speed = animator.GetBool(hashAttackMove) ? 1.5f : 0f;

                if (/*회전 조건*/ animator.GetBool(hashAttackMove))
                {
                    owner.transform.rotation = Quaternion.Lerp(owner.transform.rotation, rotation, 2f * Time.deltaTime);
                    
                    owner.Agent.Move(vecToTarget * 0.5f * Time.deltaTime);
                }                

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

                if(stateInfo.normalizedTime >= 0.5f) owner.attackBox.gameObject.SetActive(false);
                break;
        }
    }

    IEnumerator StartNextAttack()
    {
        isStop = true;
        owner.rb.useGravity = false;
        owner.rb.velocity = Vector3.zero;
        yield return new WaitForSeconds(0.5f);
        owner.animator.SetTrigger("NextAction");
        owner.rb.useGravity = true;
        owner.rb.isKinematic = false;
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (owner.MonsterViewModel.MonsterState == State.Incapacitated) return;

        if (!isNotEnd)
        {
            owner.rb.isKinematic = true;
            owner.Agent.enabled = true;
            owner.Agent.ResetPath();

            owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);
        }        
    }
}
