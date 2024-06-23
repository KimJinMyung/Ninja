using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterDetectZone : MonoBehaviour
{
    private Transform player;
    public Transform Player {  get { return player; } }

    private Transform ViewObject;

    private Monster owner;

    private Transform Eyes;

    private void Start()
    {
        owner = transform.parent.GetComponent<Monster>();

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

    private void Update()
    {
        if (ViewObject == null)
        {
            if (Player == null)
            {
                owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, null);
                return;
            }
        }

        owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, player);
    }

    private void FixedUpdate()
    {
        if (owner.MonsterViewModel.MonsterState == State.Attack) return;

        Detecting();
    }

    private void Detecting()
    {
        if (player == null) return;

        Vector3 playerDir = (player.position - transform.position).normalized;
        float angleMonAndPlayer = Vector3.Angle(Eyes.forward, playerDir);

        if(angleMonAndPlayer < owner.ViewAngle / 2f)
        {
            ViewObject = player;
            return;
        }

        ViewObject = null;
        //owner.MonsterViewModel.RequestTraceTargetChanged(owner.monsterId, null);
    }
}
