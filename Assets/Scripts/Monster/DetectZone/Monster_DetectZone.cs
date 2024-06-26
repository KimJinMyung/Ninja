using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster_DetectZone : MonoBehaviour
{
    private Transform player;

    private Monster owner;

    [SerializeField] private Transform Eyes;

    private void Awake()
    {
        owner = transform.parent.GetComponent<Monster>();
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
            owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
        }
    }

    private void Detecting()
    {
        if (player == null) return;

        Vector3 playerDir = (player.position - transform.position).normalized;
        float angleMonAndPlayer = Vector3.Angle(Eyes.forward, playerDir);

        if (angleMonAndPlayer < owner.ViewAngle / 2f)
        {
            owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
            return;
        }
            }
}
