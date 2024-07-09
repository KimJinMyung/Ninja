using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class Monster_Extension
{
    #region Monster_Type
    public static void RegisterMonsterTypeChanged(this Monster_Status_ViewModel monster_A, int acotrId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterMonsterTypeChangedCallback(acotrId, monster_A.OnResponseMonsterTypeChangedEvent, isRegister);
    }
    public static void RequestMonsterTypeChanged(this Monster_Status_ViewModel monster_A, int acotrId, MonsterType type)
    {
        ActorLogicManager._instance.OnChangedMonsterType(acotrId, type);
    }
    public static void OnResponseMonsterTypeChangedEvent(this Monster_Status_ViewModel monster_A, MonsterType type)
    {
        monster_A.MonsterType = type;
    }
    #endregion

    #region Monster_UI
    public static void BindMonsterHPChangedEvent(this Monster_Status_ViewModel input, bool isBind, int id)
    {
        MonsterManager.instance.RegisterMonsterHPChangedCallback(id, input.OnMonsterHPChanged, isBind);
    }

    public static void OnMonsterHPChanged(this Monster_Status_ViewModel input, float hp)
    {
        input.MonsterInfo.HP = hp;
        input.OnPropertyChanged(nameof(input.MonsterInfo));
        input.MonsterInfo = input.MonsterInfo;
    }

    public static void BindMonsterMaxHPChangedEvent(this Monster_Status_ViewModel input, bool isBind, int id)
    {
        MonsterManager.instance.RegisterMonsterMaxHPChangedCallback(id, input.OnMonsterMaxHPChanged, isBind);
    }

    public static void OnMonsterMaxHPChanged(this Monster_Status_ViewModel input, float maxhp)
    {
        input.MonsterInfo.MaxHP = maxhp;
        input.OnPropertyChanged(nameof(input.MonsterInfo));
        input.MonsterInfo = input.MonsterInfo;
    }
    public static void BindMonsterStaminaChangedEvent(this Monster_Status_ViewModel input, bool isBind, int id)
    {
        MonsterManager.instance.RegisterMonsterStaminaChangedCallback(id, input.OnPlayerStaminaChanged, isBind);
    }

    public static void OnPlayerStaminaChanged(this Monster_Status_ViewModel input, float stamina)
    {
        input.MonsterInfo.Stamina = stamina;
        input.OnPropertyChanged(nameof(input.MonsterInfo));
        input.MonsterInfo = input.MonsterInfo;
    }
    public static void BindPlayerMaxStaminaChangedEvent(this Monster_Status_ViewModel input, bool isBind, int id)
    {
        MonsterManager.instance.RegisterMonsterMaxStaminaChangedCallback(id, input.OnPlayerMaxStaminaChanged, isBind);
    }

    public static void OnPlayerMaxStaminaChanged(this Monster_Status_ViewModel input, float maxStamina)
    {
        input.MonsterInfo.MaxStamina = maxStamina;
        input.OnPropertyChanged(nameof(input.MonsterInfo));
        input.MonsterInfo = input.MonsterInfo;
    }
    public static void BindMonsterLifeCountChangedEvent(this Monster_Status_ViewModel input, bool isBind, int id)
    {
        MonsterManager.instance.RegisterMonsterLifeCountChangedCallback(id, input.OnMonsterLifeCountChanged, isBind);
    }

    public static void OnMonsterLifeCountChanged(this Monster_Status_ViewModel input, float lifeCount)
    {
        input.MonsterInfo.Life = lifeCount;
        input.OnPropertyChanged(nameof(input.MonsterInfo));
        input.MonsterInfo = input.MonsterInfo;
    }
    #endregion

    #region Monster_Data
    public static void RegisterMonsterInfoChanged(this Monster_Status_ViewModel monster_A, int acotrId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterInfoChangedCallback(acotrId, monster_A.OnResponseMonsterInfoChangedEvent, isRegister);
    }
    public static void RequestMonsterInfoChanged(this Monster_Status_ViewModel monster_A, int actorId, Monster_data info)
    {
        ActorLogicManager._instance.OnInfoChanged(actorId, info);
    }
    public static void OnResponseMonsterInfoChangedEvent(this Monster_Status_ViewModel monster_A, int actorId, Monster_data info)
    {
        monster_A.MonsterInfo = info;
        MonsterManager.instance.SetMonster_data(actorId, info);
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
    #region ChangedAttackMethod
    public static void RegisterAttackMethodChanged(this Monster_Status_ViewModel monster_A, int actorId, bool isRegister)
    {
        ActorLogicManager._instance.RegisterAttackMethodChangedCallback(monster_A.OnResponseAttackMethodChangedEvent, actorId, isRegister);
    }
    public static void RequestAttackMethodChanged(this Monster_Status_ViewModel monster_A, int actorId, List<Monster_Attack> attackList, Monster owner)
    {
        ActorLogicManager._instance.OnAttackMethodChanged(actorId, attackList, owner);
    }

    public static void OnResponseAttackMethodChangedEvent(this Monster_Status_ViewModel monster_A, List<Monster_Attack> attackList, Monster owner)
    {
        if(monster_A.TraceTarget == null || monster_A.CurrentAttackMethod == null)
        {
            monster_A.CurrentAttackMethod = attackList.Last();
        }
        else
        {
            float distance = Vector3.Distance(monster_A.TraceTarget.position, owner.transform.position);

            Monster_Attack closestAttackMethod = null;
            float closestDistanceDiff = float.MaxValue;

            foreach (var attackMethod in attackList)
            {
                float distanceDiff = Math.Abs(attackMethod.AttackRange - distance);

                // ��Ÿ� ���� �ְ�, �� ����� ��Ÿ��� ���� ���⸦ ����
                if (distance <= (attackMethod.AttackRange + 2.5f) && distanceDiff < closestDistanceDiff)
                {
                    closestAttackMethod = attackMethod;
                    closestDistanceDiff = distanceDiff;
                }
            }

            // ��Ÿ� ���� �ִ� ���� ����� ��Ÿ��� ���� ������� ����
            // ��Ÿ� ���� ������ ���� ����� ������ ���� �� ��Ÿ��� ���� ����
            monster_A.CurrentAttackMethod = closestAttackMethod ?? attackList.Last();
        }
    }
    #endregion
}
