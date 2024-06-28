using System;
using System.Collections.Generic;
using UnityEngine;

public enum State
{

    Idle,
    Walk,
    Run,
    Crounch,
    Jump,
    Falling,
    Climbing,
    Hide,
    Detection,  //발각되다.
    Parry,
    UsingItem,
    //공용
    Incapacitated,  //제압 당함(기절, 체간 파괴)
    Battle,
    Attack,
    Defence,
    Hurt,
    Die,
    //몬스터 전용
    Alert,
    Circling,
    RetreatAfterAttack
}

namespace ActorStateMachine
{
    public class StateMachine : MonoBehaviour
    {
        private Dictionary<string, ActorState> stateDIc = new Dictionary<string, ActorState>();
        private ActorState curState;

        public void OnStart()
        {
            if (curState == null) return;
            curState.Enter();
        }
        public void OnUpdate()
        {
            if (curState == null) return;
            curState.Update();
            curState.Transition();
        }
        public void OnLateUpdate()
        {
            if (curState == null) return;
            curState.LateUpdate();
        }
        public void OnFixedUpdate()
        {
            if (curState == null) return;
            curState.FixedUpdate();
        }
        public void InitState(string stateName)
        {
            curState = stateDIc[stateName];
        }
        public void AddState(string stateName, ActorState state)
        {
            state.SetStateMachine(this);
            stateDIc.Add(stateName, state);
        }
        public void StateUpdate()
        {
            curState.Update();
        }
        public void ChangeState(string stateName)
        {
            curState.Exit();
            curState = stateDIc[stateName];
            curState.Enter();
        }

        public void InitState<T>(T stateType) where T : Enum
        {
            InitState(stateType.ToString());
        }
        public void AddState<T>(T stateType, ActorState state) where T : Enum
        {
            AddState(stateType.ToString(), state);
        }
        public void ChangeState<T>(T stateType) where T : Enum
        {
            ChangeState(stateType.ToString());
        }
    }

    public class ActorState
    {
        private StateMachine _stateMachine;

        public void SetStateMachine(StateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        protected void ChangeState(string stateName)
        {
            _stateMachine.ChangeState(stateName);
        }

        protected void ChangeState<T>(T stateType) where T : Enum
        {
            ChangeState(stateType.ToString());
        }

        public virtual void Enter() { }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void FixedUpdate() { }
        public virtual void Exit() { }
        public virtual void Transition() { }
    }
}
