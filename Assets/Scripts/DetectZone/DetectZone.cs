using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectZone : MonoBehaviour
{
    private void OnEnable()
    {
        this.gameObject.layer = LayerMask.NameToLayer("Ignore Raycast");
        if (transform.root.CompareTag("Player"))
        {
            Rigidbody playerZone = gameObject.AddComponent<Rigidbody>();
            playerZone.useGravity = false;
            gameObject.AddComponent<PlayerLockOnZone>();                   
        }
        else
        {
            gameObject.AddComponent<MonsterDetectZone>();
        }
    }
}
