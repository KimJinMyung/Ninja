using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_FallingAssassinate : StateMachineBehaviour
{
    private Player owner;
    private Transform target;

    [SerializeField] private float speed;
    [SerializeField] private bool isStart;

    protected readonly int hashUpper = Animator.StringToHash("Upper");
    protected readonly int hashAssasinated = Animator.StringToHash("Assasinated");

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.applyRootMotion = false;
        owner = animator.GetComponent<Player>();        
        owner._velocity = 0f;
        owner.isGravityAble = true; 
        target = owner.ViewModel.AssassinatedMonsters.monster.transform;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Vector3 dir = target.position - owner.transform.position;
        dir.y = 0;
        Vector3 move = speed * dir.normalized * Time.deltaTime;

        move.y = owner._velocity * Time.deltaTime;

        owner.transform.Translate(move);
        owner.transform.rotation = Quaternion.LookRotation(target.transform.position);

        float distance = owner.transform.position.y - target.transform.position.y;
        if (distance <= 0.5f)
        {
            owner.playerController.enabled = true;
            animator.SetBool(hashUpper, false);
            target.GetComponent<Monster>().animator.SetTrigger(hashAssasinated);
        }
    }
}
