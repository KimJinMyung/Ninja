using Player_State.Extension;
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

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");

    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashParry = Animator.StringToHash("Parry");

    public override void Update()
    {
        base.Update();

        PlayerMeshAnimation();
    }

    private void PlayerMeshAnimation()
    {
        if (!owner.Animator.GetBool(hashLockOn))
        {
            //float angleValue = Vector3.Dot(owner.transform.forward, owner._moveDir.normalized * 0.3f);

            float MoveValue = Mathf.Abs(owner.InputVm.Move.y) >= 0.1f || Mathf.Abs(owner.InputVm.Move.x) >= 0.1f ? 0.25f : 0f;

            owner.Animator.SetFloat(hashMoveZ, Mathf.Lerp(owner.Animator.GetFloat(hashMoveZ), MoveValue, 3f * Time.deltaTime));
        }
        else
        {
            _playerMoveAnimation = !owner._isRun ? 3f : 5f;

            owner.Animator.SetFloat(hashMoveZ, Mathf.Lerp(owner.Animator.GetFloat(hashMoveZ), owner.InputVm.Move.y * _playerMoveAnimation, 10f * Time.deltaTime));
            owner.Animator.SetFloat(hashMoveX, Mathf.Lerp(owner.Animator.GetFloat(hashMoveX), owner.InputVm.Move.x * _playerMoveAnimation, 10f * Time.deltaTime));
        }     
    }
}

public class Player_IdleState : PlayerState
{
    public Player_IdleState(Player owner) : base(owner) { }
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
            owner.InputVm.RequestStateChanged(owner.PlayerId, State.Falling);
                //PlayerState = State.Falling;
        }
        else if(owner.InputVm.Move.magnitude > 0.1f)
        {
            owner.InputVm.RequestStateChanged(owner.PlayerId, State.Walk);
        }
    }

    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
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
            owner.InputVm.RequestStateChanged(owner.PlayerId, State.Idle);
        }
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit()
    {
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
            owner.InputVm.RequestStateChanged(owner.PlayerId, State.Idle);
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

    private float _timer;

    public override void Enter()
    {
        base.Enter();

        _timer = 0;

        owner.Animator.SetBool(hashDefence, false);
    }

    public override void Update()
    {
        base.Update();

        _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, 3f);

        if(_timer > 10f)
        {
            owner.InputVm.RequestStateChanged(owner.PlayerId, State.Idle);
        }
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() { }
}
public class AttackState : PlayerState
{
    public AttackState(Player owner) : base(owner) { }

    private float _timer;

    public override void Enter()
    {        
        base.Enter();

        _timer = 0;

        owner.Animator.SetTrigger(hashAttack);               
    }

    public override void Update()
    {
        base.Update();

        //if(owner.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.2f)
        //    owner.Animator.SetBool(hashIsMoveAble, false);

        _timer = Mathf.Clamp(_timer + Time.deltaTime, 0, owner.AttackDelay);

        if(_timer >= owner.AttackDelay)
        {
            owner.InputVm.RequestStateChanged(owner.PlayerId, State.Battle);
        }
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
        owner.Animator.SetBool(hashDefence, true);
    }

    public override void Update()
    {
        base.Update();
    }
    public override void LateUpdate() { }
    public override void FixedUpdate() { }
    public override void Exit() 
    {
        if (owner.InputVm.PlayerState == State.Parry) return;
        
        owner.Animator.SetBool(hashDefence, false);
    }
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