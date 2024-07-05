using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Monster_DetectZone : MonoBehaviour
{
    private Transform player;

    private Monster owner;

    [SerializeField] private Transform Eyes;

    private SphereCollider collider;
    private void Awake()
    {
        owner = transform.parent.GetComponent<Monster>();
        collider = GetComponent<SphereCollider>();    
    }

    private void OnEnable()
    {
        DefaultDetectRange();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void DefaultDetectRange()
    {
        collider.radius = owner.MonsterViewModel.MonsterInfo.ViewRange;
    }

    private void OnTriggerExit(Collider other)
    {
        if(owner.Type == monsterType.Boss) return;

        if (other.CompareTag("Player"))
        {
            if(Vector3.Distance(other.transform.position, transform.position) > collider.radius)
            {
                player = null;
                owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
            }
        }
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.red;
    //    Gizmos.DrawWireSphere(owner.transform.position, collider.radius);
    //}

    private void Update()
    {
        if(collider.transform.position != Eyes.position)
            collider.transform.position = Eyes.position;

        if (owner.MonsterViewModel.TraceTarget != null) collider.radius = owner.MonsterViewModel.MonsterInfo.ViewRange + 3f;
        else if (!collider.radius.Equals(owner.MonsterViewModel.MonsterInfo.ViewRange)) DefaultDetectRange();
    }

    private void FixedUpdate()
    {
         Detecting();
    }

    [SerializeField] private GameObject aa;

    private void Detecting()
    {
        if (player == null) return;

        Vector3 playerDir = (player.transform.position - transform.position).normalized;
        float angleMonAndPlayer = Vector3.Angle(Eyes.forward, playerDir);

        float viewAngle = owner.MonsterViewModel.MonsterInfo.ViewAngel;

        if (owner.MonsterViewModel.TraceTarget == null)
        {
            if (angleMonAndPlayer < viewAngle / 2f)
            {
                owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
            }
        }
    }
}
