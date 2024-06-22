using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Monster_Extension
{
    #region HP
    public static void RegisterHPChanged(this Monster_Status_ViewModel monster_A,int acotrId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterHpChangedCallback(acotrId, monster_A.OnResponseHPChangedEvent, isRegister);
    }
    public static void RequestHPChanged(this Monster_Status_ViewModel monster_A, int acotrId, float damage)
    {
        ActorLogicManager._instance.OnHpChanged(acotrId, damage);
    }
    public static void OnResponseHPChangedEvent(this Monster_Status_ViewModel monster_A, float damage)
    {
        monster_A.HP = Mathf.Clamp(monster_A.HP - damage, 0, monster_A.HP);
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
