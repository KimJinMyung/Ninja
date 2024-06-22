using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectZone : MonoBehaviour
{
    PlayerInput _playerInput;
    InputActionMap actionMap;
    InputAction lockOnAction;
    PlayerLockOnZone _playerLockOnZone;

    private void OnEnable()
    {
        if (transform.root.CompareTag("Player"))
        {
            Rigidbody playerZone = gameObject.AddComponent<Rigidbody>();
            playerZone.useGravity = false;
            _playerLockOnZone = gameObject.AddComponent<PlayerLockOnZone>();            
            _playerInput = transform.root.GetComponent<PlayerInput>();
            actionMap = _playerInput.actions.FindActionMap("Player");
            lockOnAction = actionMap.FindAction("LockOn");
            lockOnAction.performed += _playerLockOnZone.OnLockOnMode;
        }
        else
        {
            gameObject.AddComponent<MonsterDetectZone>();
        }
    }

    private void OnDisable()
    {
        if (transform.root.CompareTag("Player"))
        {
            lockOnAction.performed -= _playerLockOnZone.OnLockOnMode;
        }        
    }
}
