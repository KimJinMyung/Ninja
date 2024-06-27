using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Monster_Extension
{
    #region Monster_Type
    public static void RegisterMonsterTypeChanged(this Monster_Status_ViewModel monster_A, int acotrId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterMonsterTypeChangedCallback(acotrId, monster_A.OnResponseMonsterTypeChangedEvent, isRegister);
    }
    public static void RequestMonsterTypeChanged(this Monster_Status_ViewModel monster_A, int acotrId, monsterType type)
    {
        ActorLogicManager._instance.OnChangedMonsterType(acotrId, type);
    }
    public static void OnResponseMonsterTypeChangedEvent(this Monster_Status_ViewModel monster_A, monsterType type)
    {
        monster_A.MonsterType = type;
    }
    #endregion

    #region Monster_Data
    public static void RegisterMonsterInfoChanged(this Monster_Status_ViewModel monster_A, int acotrId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterInfoChangedCallback(acotrId, monster_A.OnResponseMonsterInfoChangedEvent, isRegister);
    }
    public static void RequestMonsterInfoChanged(this Monster_Status_ViewModel monster_A, int acotrId, Monster_data info)
    {
        ActorLogicManager._instance.OnInfoChanged(acotrId, info);
    }
    public static void OnResponseMonsterInfoChangedEvent(this Monster_Status_ViewModel monster_A, Monster_data info)
    {
        monster_A.MonsterInfo = info;
    }
    #endregion

    #region State
    public static void RegisterStateChanged(this Monster_Status_ViewModel monster_A, int ActirId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterStateChangedCallback(ActirId, monster_A.OnResponseStateChangedEvent, isRegister);
    }
    public static void RequestStateChanged(this Monster_Status_ViewModel monster_A, int ActirId, State state)
    {
        ActorLogicManager._instance.OnChangedState(ActirId, state);
    }
    public static void OnResponseStateChangedEvent(this Monster_Status_ViewModel monster_A, State state)
    {
        monster_A.MonsterState = state;
    }
    #endregion
    #region Target
    public static void RegisterTraceTargetChanged(this Monster_Status_ViewModel monster_A, int actorId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterTraceTargetChangedCallback(monster_A.OnResponseTraceTargetChangedEvent, actorId,isRegister);
    }
    public static void RequestTraceTargetChanged(this Monster_Status_ViewModel monster_A, int actorId, Transform traceTarget)
    {
        ActorLogicManager._instance.OnTraceTarget(actorId, traceTarget);
    }
    public static void OnResponseTraceTargetChangedEvent(this Monster_Status_ViewModel monster_A, Transform traceTarget)
    {
        monster_A.TraceTarget = traceTarget;    
    }
    #endregion
}
