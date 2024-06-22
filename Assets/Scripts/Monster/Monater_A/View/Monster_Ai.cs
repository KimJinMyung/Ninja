using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Monster_Ai : MonoBehaviour
{
    private GameObject _detectZone;
    public GameObject DetectZone { get { return _detectZone; } set { _detectZone = value; } }

    private Monster owner;

    private NavMeshAgent agent;
    private Animator animator;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();

        owner = GetComponent<Monster>();
    }

    private void OnEnable()
    {
        GameObject detectZone = Instantiate(_detectZone);     
        detectZone.transform.parent = transform;
    }

    private void Update()
    {
        if(owner.MonsterViewModel.MonsterState == State.Trace)
        {
            agent.SetDestination(owner.MonsterViewModel.TraceTarget.position);
        }
    }
}
