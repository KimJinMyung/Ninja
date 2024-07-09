using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Debug_Teleport : MonoBehaviour
{

    [SerializeField] Transform WarpPosition;

    CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    public void OnDebugWarp(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            characterController.enabled = false;

            transform.position = WarpPosition.position;

            characterController.enabled = true;
        }
    }
}
