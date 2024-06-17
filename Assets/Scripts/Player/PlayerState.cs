using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player의 동작 제어

public class PlayerState : ActorState
{    
    protected Player owner;

    public PlayerState(Player owner)
    {
        this.owner = owner;
    }

    protected float _playerMoveAnimation;

    protected readonly int hashMoveZ = Animator.StringToHash("Z_Value");
    protected readonly int hashMoveX = Animator.StringToHash("X_Value");

    public override void Update()
    {
        base.Update();
        owner.Animator.SetFloat(hashMoveZ, Mathf.Lerp(owner.Animator.GetFloat(hashMoveZ), owner.InputVm.Move.y * _playerMoveAnimation, 10f * Time.deltaTime));
        owner.Animator.SetFloat(hashMoveX, Mathf.Lerp(owner.Animator.GetFloat(hashMoveX), owner.InputVm.Move.x * _playerMoveAnimation, 10f * Time.deltaTime));
    }
}

public class IdleState : PlayerState
{
    public IdleState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        _playerMoveAnimation = 0f;
    }

    public override void Update()
    {
        base.Update();
        if(ActorLogicManager._instance.OnChangedStateFalling(owner.transform,owner.MaxDistance, owner.GroundLayer))
        {
            owner.PlayerState = State.Falling;
        }
        else if(owner.InputVm.Move.magnitude > 0.1f)
        {
            owner.PlayerState = State.Walk;
        }
    }

    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}

public class WalkState : PlayerState
{
    public WalkState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        _playerMoveAnimation = 3f;
    }

    public override void Update()
    {
        base.Update();

        if (owner.InputVm.Move.magnitude <= 0.1f)
        {
            owner.PlayerState = State.Idle;
        }
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit()
    {
        Debug.Log("Exiting Idle State");
    }
}
public class RunState : PlayerState
{
    public RunState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}

public class CrouchState : PlayerState
{
    public CrouchState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class JumpState : PlayerState
{
    public JumpState(Player owner) : base(owner) { }    
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class FallingState : PlayerState
{
    public FallingState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if (!ActorLogicManager._instance.OnChangedStateFalling(owner.transform, owner.MaxDistance, owner.GroundLayer))
        {
            owner.PlayerState = State.Idle;
        }
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() 
    {

    }
    public override void Exit() { }
}
public class ClimbingState : PlayerState
{
    public ClimbingState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class HideState : PlayerState
{
    public HideState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class DetectionState : PlayerState
{
    public DetectionState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class BattleState : PlayerState
{
    public BattleState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class AttackState : PlayerState
{
    public AttackState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class DefenceState : PlayerState
{
    public DefenceState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class ParryState : PlayerState
{
    public ParryState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class IncapacitatedState : PlayerState
{
    public IncapacitatedState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class UsingItemState : PlayerState
{
    public UsingItemState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class DieState : PlayerState
{
    public DieState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}