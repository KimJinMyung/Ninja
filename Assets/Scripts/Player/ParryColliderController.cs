using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParryColliderController : MonoBehaviour
{
    [Header("Parry Box")]
    [SerializeField] private BoxCollider _parryCollider;
    public BoxCollider parryCollider { get { return _parryCollider; } }

    private void Awake()
    {
        _parryCollider.enabled = false;
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
