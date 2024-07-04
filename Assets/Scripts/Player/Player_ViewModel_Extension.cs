using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Player_ViewModel_Extension
{
    #region State
    public static void RegisterStateChanged(this Player_ViewModel input, int ActirId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterStateChangedCallback(ActirId, input.OnResponseStateChangedEvent, isRegister);
    }
    public static void RequestStateChanged(this Player_ViewModel input, int ActirId, State state)
    {
        ActorLogicManager._instance.OnChangedState(ActirId, state);
    }
    public static void OnResponseStateChangedEvent(this Player_ViewModel input, State state)
    {
        input.playerState = state;
    }
    #endregion
    #region PlayerInfo
    public static void RegisterPlayerDataChanged(this Player_ViewModel input, int ActirId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterPlayerDataChangedCallback(ActirId, input.OnResponsePlayerDataChangedEvent, isRegister);
    }
    public static void RequestPlayerDataChanged(this Player_ViewModel input, int ActirId, Player_data data)
    {
        ActorLogicManager._instance.OnChangedPlayerData(ActirId, data);
    }
    public static void OnResponsePlayerDataChangedEvent(this Player_ViewModel input, Player_data data)
    {
        input.playerInfo = data;
    }
    #endregion
    #region Move
    public static void RegisterMoveVelocity(this Player_ViewModel input, bool isRegister)
    {
        ActorLogicManager._instance.RegisterMoveVelocityChangedCallback(input.OnResponseMoveVelocityChangedEvent, isRegister);
    }

    public static void RequestMoveOnInput(this Player_ViewModel input, float x, float y)
    {
        ActorLogicManager._instance.OnMoveInput(x, y);
    }

    public static void OnResponseMoveVelocityChangedEvent(this Player_ViewModel input, float contextValueX, float contextValueY)
    {
        input.Move = new Vector2(contextValueX, contextValueY);
    }
    #endregion
    #region ActorRotate
    public static void RegisterActorRotate(this Player_ViewModel input, bool isRegister)
    {
        ActorLogicManager._instance.RegisterActorRotateChangedCallback(input.OnResponseActorRotateChangedEvent, isRegister);
    }
    public static void RequestActorRotate(this Player_ViewModel input, float x, float y, float z)
    {
        ActorLogicManager._instance.OnActorRotate(x, y, z);
    }

    public static void OnResponseActorRotateChangedEvent(this Player_ViewModel input, float contextValueX, float contextValueY, float contextValueZ)
    {
        input.Rotation = Quaternion.Euler(new Vector3(contextValueX, contextValueY, contextValueZ));
    }
    #endregion
    #region LockOnTarget
    public static void ReigsterLockOnTargetChanged(this Player_ViewModel input, bool isRegister)
    {
        ActorLogicManager._instance.RegisterLockOnTargetChangedCallback(input.OnResponseLockOnTargetChangedEvent, isRegister);
    }

    public static void RequestLockOnTarget(this Player_ViewModel input, Transform target)
    {
        ActorLogicManager._instance.OnLockOnTarget(target);
    }

    public static void OnResponseLockOnTargetChangedEvent(this Player_ViewModel input, Transform tartget)
    {
        input.LockOnTarget = tartget;        
    }
    #endregion
    #region Assassinated
    public static void ReigsterAssassinatedTypeChanged(this Player_ViewModel input, bool isRegister)
    {
        ActorLogicManager._instance.RegisterAssassinatedChangedCallback(input.OnResponseAssassinatedTypeChangedEvent, isRegister);
    }

    public static void RequestAssassinatedType(this Player_ViewModel input, AssassinatedType type, Monster monster)
    {
        ActorLogicManager._instance.OnAssassinated(type, monster);
    }

    public static void OnResponseAssassinatedTypeChangedEvent(this Player_ViewModel input, AssassinatedType type, Monster monster)
    {
        input.AssassinatedMonsters.monster = monster;
        input.AssassinatedMonsters.Type = type;
    }
    #endregion
}
