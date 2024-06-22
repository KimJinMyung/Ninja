using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetectZone : MonoBehaviour
{
    private Transform player;

    private Monster owner;
    private SphereCollider zoneCollider;

    private void OnEnable()
    {
        owner = transform.root.GetComponent<Monster>();
        zoneCollider = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.transform;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
        }
    }

    private void FixedUpdate()
    {
        if (owner.MonsterViewModel.MonsterState == State.Attack) return;

        Detecting();
        //if (Detecting())
        //{
        //    if(owner.MonsterViewModel.MonsterState != State.Battle)
        //    {
        //        owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
        //        owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Trace);
        //    }            
        //}
        //else if(owner.MonsterViewModel.MonsterState == State.Trace)
        //{
        //    owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, null);
        //    owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Alert);
        //}
        //else if(owner.MonsterViewModel.MonsterState != State.Alert)
        //{            
        //    owner.MonsterViewModel.RequestStateChanged(owner.monsterId, State.Idle);
        //}
    }

    private void Detecting()
    {
        if (player == null) return;

        Vector3 playerDir = (player.position - transform.position).normalized;
        float angleMonAndPlayer = Vector3.Angle(transform.forward, playerDir);

        if(angleMonAndPlayer < owner.ViewAngle / 2f)
        {
            if(Physics.Raycast(transform.position, playerDir, out RaycastHit hit, zoneCollider.radius))
            {
                if(hit.transform == player)
                {
                    owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
                }
            }
        }

        owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, null);
    }
}
