using UnityEngine;
using UnityEngine.InputSystem;

namespace Player_State.Extension
{
    public static class PlayerState_Extension
    {
        public static void RegisterMoveVelocity(this Player_Input input, bool isRegister)
        {
            ActorLogicManager._instance.RegisterMoveVelocityChangedCallback(input.OnResponseMoveVelocityChangedEvent, isRegister);
        }

        public static void OnResponseMoveVelocityChangedEvent(this Player_Input input, InputAction.CallbackContext context)
        {
            input.Move = context.ReadValue<Vector2>();
        }
    }
}
