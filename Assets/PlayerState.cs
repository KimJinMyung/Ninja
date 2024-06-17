using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Player의 동작 제어

public class PlayerState : MonoBehaviour
{
    public class IdleState : ActorState
    {
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
        public override void Exit()
        {
            Debug.Log("Exiting Idle State");
        }
    }

    public class WalkState : ActorState
    {
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
        public override void Exit()
        {
            Debug.Log("Exiting Idle State");
        }
    }
    public class RunState : ActorState
    {
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

    public class CrouchState : ActorState
    {
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
    public class JumpState : ActorState
    {
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
    public class FallingState : ActorState
    {
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
    public class ClimbingState : ActorState
    {
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
    public class HideState : ActorState
    {
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
    public class DetectionState : ActorState
    {
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
    public class BattleState : ActorState
    {
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
    public class AttackState : ActorState
    {
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
    public class DefenceState : ActorState
    {
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
    public class ParryState : ActorState
    {
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
    public class IncapacitatedState : ActorState
    {
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
    public class UsingItemState : ActorState
    {
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
    public class DieState : ActorState
    {
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
}
