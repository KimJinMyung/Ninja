using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetectZone : MonoBehaviour
{
    private Transform player;

    private Monster owner;
    private SphereCollider zoneCollider;

    private Transform Eyes;

    private void Start()
    {
        owner = transform.parent.GetComponent<Monster>();
        zoneCollider = GetComponent<SphereCollider>();

        FindEyeTransform(owner.transform);
    }

    private void FindEyeTransform(Transform parent)
    {
        foreach(Transform child in parent)
        {
            if (child.CompareTag("Eye"))
            {
                this.Eyes = child;
                break;
            }
            FindEyeTransform(child);
        }
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

        if (owner.MonsterViewModel.TraceTarget != null)
            Debug.Log(owner.MonsterViewModel.TraceTarget);
    }

    private void Detecting()
    {
        if (player == null) return;

        Vector3 playerDir = (player.position - transform.position).normalized;
        float angleMonAndPlayer = Vector3.Angle(Eyes.forward, playerDir);

        if(angleMonAndPlayer < owner.ViewAngle / 2f)
        {
            owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
            return;
        }

        owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, null);
    }
}
