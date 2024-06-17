using UnityEngine;
using UnityEngine.InputSystem;

namespace Player_State.Extension
{
    public static class PlayerState_Extension
    {
        public static void RegisterMoveVelocity(this InputViewModel input, bool isRegister)
        {
            ActorLogicManager._instance.RegisterMoveVelocityChangedCallback(input.OnResponseMoveVelocityChangedEvent, isRegister);            
        }

        public static void RequestMoveOnInput(this InputViewModel input, float x, float y)
        {
            ActorLogicManager._instance.OnMoveInput(x,y);
        }

        public static void OnResponseMoveVelocityChangedEvent(this InputViewModel input, float contextValueX, float contextValueY)
        {
            input.Move = new Vector2(contextValueX, contextValueY);
        }
    }
}
