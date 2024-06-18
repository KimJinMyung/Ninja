using Cinemachine;
using UnityEngine;

namespace Player_State.Extension
{
    public static class PlayerState_Extension
    {
        #region State
        public static void RegisterStateChanged(this InputViewModel input, bool isRegister)
        {
            ActorLogicManager._instance.RegisterStateChangedCallback(input.OnResponseStateChangedEvent, isRegister);
        }
        public static void RequestStateChanged(this InputViewModel input, State state)
        {
            ActorLogicManager._instance.OnChangedState(state);
        }
        public static void OnResponseStateChangedEvent(this InputViewModel input, State state)
        {
            input.PlayerState = state;
        }
        #endregion

        #region Move
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
        #endregion

        #region ActorRotate
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
        #endregion

        #region LockOn
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
        #endregion
    }
}
