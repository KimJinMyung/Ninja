using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shuriken : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float AddForce;

    public Monster owner { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        rb.AddForce(transform.forward * AddForce, ForceMode.Impulse);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == owner.gameObject) return;
        //데미지 부여

        if (other.CompareTag("Player"))
        {
            Player hitTarget = other.GetComponent<Player>();
            hitTarget.Hurt(owner, owner.MonsterViewModel.MonsterInfo.ATK);
            Destroy(this.gameObject);
        }        
    }

    public void SetShooterData(Monster monster)
    {
        owner = monster;
    }
}
