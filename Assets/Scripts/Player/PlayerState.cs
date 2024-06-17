using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player�� ���� ����

public class PlayerState : ActorState
{    
    protected Player owner;

    public PlayerState(Player owner)
    {
        this.owner = owner;
    }


}

public class IdleState : PlayerState
{
    public IdleState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
        if(ActorLogicManager._instance.OnChangedStateFalling(owner.transform,owner.MaxDistance, owner.GroundLayer))
        {
            owner.PlayerState = State.Falling;
        }
        else if(owner.Velocity.magnitude > 0.1f)
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
    }

    public override void Update()
    {
        base.Update();

        if (owner.Velocity.magnitude < 0.1f)
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
    public override void FixedUpdate() { }
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