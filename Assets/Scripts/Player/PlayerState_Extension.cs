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

        public static void OnResponseActorRotateChangedEvent(this InputViewModel input, float contextValueX,float contextValueY, float contextValueZ)
        {
            input.Rotation = Quaternion.Euler(new Vector3(contextValueX, contextValueY, contextValueZ));
        }

        public static void RegisterMousePosition(this InputViewModel input, bool isRegister)
        {
            ActorLogicManager._instance.RegisterMousePositionChangedCallback(input.OnResponseMousePositionChangedEvent, isRegister);
        }
        public static void RequestMousePosition(this InputViewModel input, float x, float y)
        {
            ActorLogicManager._instance.OnMousePostition(x, y);
        }

        public static void OnResponseMousePositionChangedEvent(this InputViewModel input, float contextValueX, float contextValueY)
        {
            input.MousePosition = new Vector2(contextValueX, contextValueY);
        }

    }
}
