using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Atk_Shuriken : MonoBehaviour,IArk
{
    public float attackRange { get; set; }
    public float attackSpeed { get; set; }

    private void Start()
    {
        attackRange = 10f;
        attackSpeed = 1.0f;
    }
}
