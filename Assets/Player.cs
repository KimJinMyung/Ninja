using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private StateMachine _stateMachine;

    private void Awake()
    {
        _stateMachine = gameObject.AddComponent<StateMachine>();

        _stateMachine.AddState(State.Idle, new PlayerState.IdleState());
        _stateMachine.AddState(State.Walk, new PlayerState.WalkState());
        _stateMachine.AddState(State.Run, new PlayerState.RunState());
        _stateMachine.AddState(State.Crounch, new PlayerState.CrouchState());
        _stateMachine.AddState(State.Jump, new PlayerState.JumpState());
        _stateMachine.AddState(State.Flling, new PlayerState.FallingState());
        _stateMachine.AddState(State.Climbing, new PlayerState.ClimbingState());
        _stateMachine.AddState(State.Hide, new PlayerState.HideState());
        _stateMachine.AddState(State.Detection, new PlayerState.DetectionState());
        _stateMachine.AddState(State.Battle, new PlayerState.BattleState());
        _stateMachine.AddState(State.Attack, new PlayerState.AttackState());
        _stateMachine.AddState(State.Defence, new PlayerState.DefenceState());
        _stateMachine.AddState(State.Parry, new PlayerState.ParryState());
        _stateMachine.AddState(State.Incapacitated, new PlayerState.IncapacitatedState());
        _stateMachine.AddState(State.UsingItem, new PlayerState.UsingItemState());
        _stateMachine.AddState(State.Die, new PlayerState.DieState());

        _stateMachine.InitState(State.Idle);
    }

    private void Start()
    {
        _stateMachine.OnStart();
    }

    private void Update()
    {
        _stateMachine.OnUpdate();
    }

    private void LateUpdate()
    {
        _stateMachine.OnLateUpdate();
    }
    private void FixedUpdate()
    {
        _stateMachine.OnFixedUpdate();
    }
}
