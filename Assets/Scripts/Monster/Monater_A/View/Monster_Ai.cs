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
        Instantiate(_detectZone);     
    }

    //IEnumerator StartAI()
    //{
    //    while (owner.HP > 0)
    //    {
    //        yield return new WaitForSeconds(0.3f);

    //        owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);
    //    }
    //    yield return null;
    //}
    
}
