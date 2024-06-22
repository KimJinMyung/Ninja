using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectZone : MonoBehaviour
{
    private void OnEnable()
    {
        if (transform.root.CompareTag("Player"))
        {
            Rigidbody playerZone = gameObject.AddComponent<Rigidbody>();
            playerZone.useGravity = false;
            var _playerLockOnZone = gameObject.AddComponent<PlayerLockOnZone>();            
            var _playerInput = transform.root.GetComponent<PlayerInput>();
            var actionMap = _playerInput.actions.FindActionMap("Player");
            var lockOnAction = actionMap.FindAction("LockOn");
            lockOnAction.performed += _playerLockOnZone.OnLockOnMode;
        }
    }

    private void OnDisable()
    {
        var _playerInput = GetComponent<PlayerInput>();
        var actionMap = _playerInput.actions.FindActionMap("Player");
        var lockOnAction = actionMap.FindAction("LockOn");
        var _playerLockOnZone = GetComponent<PlayerLockOnZone>();
        lockOnAction.performed -= _playerLockOnZone.OnLockOnMode;
    }
}
