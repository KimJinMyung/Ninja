using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleColliderController : MonoBehaviour
{
    [Header("Parry Box")]
    [SerializeField] private BoxCollider _parryCollider;
    public BoxCollider parryCollider { get { return _parryCollider; } }

    [Header("Attack Collider")]
    [SerializeField] private BoxCollider _attackZone;
    public BoxCollider AttackZone { get { return _attackZone; } }

    private void Awake()
    {
        _parryCollider.enabled = false;
        _attackZone.enabled = false;
    }

    public void AttackColliderOn()
    {
        if(!_attackZone.enabled)
            _attackZone.enabled = true;

        Debug.Log("Attack On");
    }

    public void AttackColliderOff()
    {
        if(_attackZone.enabled)
            _attackZone.enabled = false;

        Debug.Log("Attack Off");
    }

    public void ParryColliderOn()
    {
        _parryCollider.enabled = true;
    }

    public void ParryColliderOFF()
    {
        _parryCollider.enabled = false;
    }
}
