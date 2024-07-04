using UnityEngine;
using ActorStateMachine;
using System.Runtime.CompilerServices;
using UnityEditor.SceneTemplate;
using static UnityEditor.Profiling.HierarchyFrameDataView;
using System.Threading;

//Player의 동작 제어
public class PlayerState : ActorState
{
    protected Player owner;
    public PlayerState(Player owner)
    {
        this.owner = owner;
    }

    protected readonly int hashMoveZ = Animator.StringToHash("Z_Value");
    protected readonly int hashMoveX = Animator.StringToHash("X_Value");

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");
    protected readonly int hashAttack = Animator.StringToHash("Attack");
    protected readonly int hashAttackAble = Animator.StringToHash("IsAttackAble");
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");

    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashDefenceHit = Animator.StringToHash("DefenceHit");
    protected readonly int hashParry = Animator.StringToHash("Parry");
    protected readonly int hashBattleMode = Animator.StringToHash("BattleMode");
    protected readonly int hashBattleModeChanged = Animator.StringToHash("BattleModeChanged");

    protected readonly int hashClimbing = Animator.StringToHash("Climbing");

    protected readonly int hashHurt = Animator.StringToHash("Hurt");
    protected readonly int hashDie = Animator.StringToHash("Die");

    protected readonly int hashAssasinated = Animator.StringToHash("Assasinated");
    protected readonly int hashForward = Animator.StringToHash("Forward");
    protected readonly int hashUpper = Animator.StringToHash("Upper");
    protected readonly int hashGrounded = Animator.StringToHash("IsGround");

    public override void Update()
    {
        base.Update();
        Debug.Log(owner.ViewModel.playerState);
        PlayerMeshAnimation();
    }

    private void PlayerMeshAnimation()
    {
        if (!owner.Animator.GetBool(hashLockOn))
        {
            //float angleValue = Vector3.Dot(owner.transform.forward, owner._moveDir.normalized * 0.3f);

            float MoveValue = Mathf.Abs(owner.ViewModel.Move.y) < 0.1f && Mathf.Abs(owner.ViewModel.Move.x) < 0.1f ? 0f : owner._isRun? 3f : 1f;

            owner.Animator.SetFloat(hashMoveZ, Mathf.Lerp(owner.Animator.GetFloat(hashMoveZ), MoveValue, 10f * Time.deltaTime));
        }
        else
        {
            float MoveValue = Mathf.Abs(owner.ViewModel.Move.y) < 0.1f && Mathf.Abs(owner.ViewModel.Move.x) < 0.1f ? 0f : owner._isRun? 3f : 1f;          

            owner.Animator.SetFloat(hashMoveZ, Mathf.Lerp(owner.Animator.GetFloat(hashMoveZ), owner.ViewModel.Move.y * MoveValue, 10f * Time.deltaTime));
            owner.Animator.SetFloat(hashMoveX, Mathf.Lerp(owner.Animator.GetFloat(hashMoveX), owner.ViewModel.Move.x * MoveValue, 10f * Time.deltaTime));
        }
    }
}

public class Player_IdleState : PlayerState
{
    public Player_IdleState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
    }

    public override void Update()
    {
        base.Update();
    }
}

public class CrouchState : PlayerState
{
    public CrouchState(Player owner) : base(owner) { }

}
public class JumpState : PlayerState
{
    public JumpState(Player owner) : base(owner) { }
}
public class FallingState : PlayerState
{
    public FallingState(Player owner) : base(owner) { }

    public override void Update()
    {
        base.Update();
    }
}
public class ClimbingState : PlayerState
{
    public ClimbingState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        owner.isGravityAble = false;
        owner.Animator.SetBool(hashClimbing, true);
    }

    public override void Exit()
    {
        base.Exit();
        owner.isGravityAble = true;
        owner.Animator.SetBool(hashClimbing, false);
    }
}
public class HideState : PlayerState
{
    public HideState(Player owner) : base(owner) { } 
}

public class DetectionState : PlayerState
{
    public DetectionState(Player owner) : base(owner) { }
}
public class BattleState : PlayerState
{
    public BattleState(Player owner) : base(owner) { }

    private float _timer;

    public override void Enter()
    {
        base.Enter();

        _timer = 0;
        owner.Animator.SetBool(hashAttackAble, true);
        owner.Animator.SetBool(hashIsMoveAble, true);
        //owner.Animator.SetBool(hashDefence, false);
    }

    public override void Update()
    {
        base.Update();

        _timer = Mathf.Clamp(_timer + Time.deltaTime, 0f, 5f);

        if (_timer >= 5f)
        {
            owner.Animator.SetTrigger(hashBattleModeChanged);
            owner.ViewModel.RequestStateChanged(owner.player_id, State.Idle);
            return;
        }
    }    
}
public class AttackState : PlayerState
{
    public AttackState(Player owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();

        owner.Animator.SetBool(hashIsMoveAble, false);
    }
}
public class AssassinatedState : PlayerState
{    
    public AssassinatedState(Player owner) : base(owner) { }

    private AssassinationData _data;

    public override void Enter()
    {
        base.Enter();
        owner.isGravityAble = false;
        owner.playerController.enabled = false;
        _data = owner.ViewModel.AssassinatedMonsters;

        switch (_data.Type)
        {
            case AssassinatedType.Forward:
                owner.transform.position = _data.monster.transform.position + _data.monster.transform.forward * 3f;
                //전방에서 몬스터를 즉사시키는 모션 실행
                owner.Animator.SetBool(hashUpper, false);
                owner.Animator.SetBool(hashForward, true);
                _data.monster.animator.SetBool(hashForward, true);

                owner.Animator.SetTrigger(hashAssasinated);
                _data.monster.animator.SetTrigger(hashAssasinated);
                break;
            case AssassinatedType.Backward:
                owner.transform.position = _data.monster.transform.position - _data.monster.transform.forward * 0.8f;
                //등 뒤에서 몬스터를 즉사시키는 모션 실행
                owner.Animator.SetBool(hashUpper, false);
                owner.Animator.SetBool(hashForward, false);
                _data.monster.animator.SetBool(hashForward, false);

                owner.Animator.SetTrigger(hashAssasinated);
                _data.monster.animator.SetTrigger(hashAssasinated);
                break;
            case AssassinatedType.UpForward:
                owner.Animator.SetBool(hashGrounded, owner.isGround);
                owner.Animator.SetBool(hashUpper, true);
                _data.monster.animator.SetBool(hashUpper, true);

                owner.Animator.SetBool(hashForward, true);
                _data.monster.animator.SetBool(hashForward, true);

                owner.Animator.SetTrigger(hashAssasinated);
                //_data.monster.animator.SetTrigger(hashAssasinated);
                break;
            case AssassinatedType.UpBackward:
                owner.Animator.SetBool(hashGrounded, owner.isGround);
                owner.Animator.SetBool(hashUpper, true);
                _data.monster.animator.SetBool(hashUpper, true);

                owner.Animator.SetBool(hashForward, false);
                _data.monster.animator.SetBool(hashForward, false);

                owner.Animator.SetTrigger(hashAssasinated);
                //_data.monster.animator.SetTrigger(hashAssasinated);
                break;
        }

        Debug.Log(_data.monster.name);
        owner.transform.rotation = Quaternion.LookRotation(_data.monster.transform.position);
    }

    public override void Exit()
    {
        base.Exit();
        owner.playerController.enabled = true;
    }
}
public class DefenceState : PlayerState
{
    public DefenceState(Player owner) : base(owner) { }

    private int DefenceLayerIndex;
    public override void Enter()
    {
        base.Enter();
        owner.Animator.SetBool(hashIsMoveAble, false);
        DefenceLayerIndex = owner.Animator.GetLayerIndex("Defence");
        owner.Animator.SetLayerWeight(DefenceLayerIndex, 1);
        owner.Animator.SetTrigger(hashHurt);
    }

    public override void Exit()
    {
        base.Exit();
        owner.Animator.SetBool(hashIsMoveAble, true);
    }
}
public class ParryState : PlayerState
{
    public ParryState(Player owner) : base(owner) { }

    public override void Update()
    {
        base.Update();
    }
}
public class IncapacitatedState : PlayerState
{
    public IncapacitatedState(Player owner) : base(owner) { }

}
public class UsingItemState : PlayerState
{
    public UsingItemState(Player owner) : base(owner) { }

}

public class GrapplingState : PlayerState
{
    public GrapplingState(Player owner) : base(owner) { }

}

public class HurtState : PlayerState
{
    public HurtState(Player owner) : base(owner) { }

    public override void Enter()
    {
        base.Enter();
        owner.Animator.SetTrigger(hashHurt);
        owner.Animator.SetBool(hashAttackAble, false);
    }

    public override void Exit()
    {
        base.Exit();
        owner.Animator.SetBool(hashAttackAble, true);
    }
}
public class DieState : PlayerState
{
    public DieState(Player owner) : base(owner) { }
    public override void Enter()
    {
        base.Enter();
        owner.Animator.SetBool(hashDie, true);
    }

    public override void Exit()
    {
        base.Exit();
        owner.Animator.SetBool(hashDie, false);
    }
}