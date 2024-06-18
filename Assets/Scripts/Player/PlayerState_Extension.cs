using Cinemachine;
using UnityEngine;

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

        public static void RegisterActorRotate(this InputViewModel input, bool isRegister)
        {
            ActorLogicManager._instance.RegisterActorRotateChangedCallback(input.OnResponseActorRotateChangedEvent, isRegister);
        }
        public static void RequestActorRotate(this InputViewModel input, float x, float y, float z)
        {
            ActorLogicManager._instance.OnActorRotate(x, y, z);
        }

        public static void OnResponseActorRotateChangedEvent(this InputViewModel input, float contextValueX, float contextValueY, float contextValueZ)
        {
            input.Rotation = Quaternion.Euler(new Vector3(contextValueX, contextValueY, contextValueZ));
        }

        public static void ReigsterIsLockOn(this InputViewModel input, bool isRegister)
        {
            ActorLogicManager._instance.RegisterIsLockOnModeChangedCallback(input.OnResponseIsLockOnChangedEvent, isRegister);
        }

        public static void RequstIsLockOn(this InputViewModel input, bool isLockOn)
        {
            ActorLogicManager._instance.OnIsLockOn(isLockOn);
        }

        public static void OnResponseIsLockOnChangedEvent(this InputViewModel input, bool isLockOn)
        {
            input.IsLockOnMode = isLockOn;
        }
    }
}
