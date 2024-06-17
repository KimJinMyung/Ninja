using Player_State.Extension;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayer;

    [SerializeField] private float moveSpeed;

    private InputViewModel _inputVm;

    public float MaxDistance
    {
        get { return maxDistance; }
    }

    public LayerMask GroundLayer
    {
        get { return groundLayer; }
    }

    [Header("Debug")]
    [SerializeField] private bool _debug;

    public bool _Debug
    {
        get { return  _debug; }
    }

    private StateMachine _stateMachine;
    private State _playerState;

    public State PlayerState
    {
        get
        {
            return _playerState;
        }
        set
        {
            if( _playerState == value ) return;

            _playerState = value;
            _stateMachine.ChangeState(_playerState);
        }
    }

    public StateMachine StateMachine
    {
        get
        {
            return _stateMachine;
        }
    }

    private CharacterController _characterController;

    public Vector3 Velocity
    {
        get
        {
            return _characterController.velocity;
        }
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _stateMachine = gameObject.AddComponent<StateMachine>();

        _stateMachine.AddState(State.Idle, new IdleState(this));
        _stateMachine.AddState(State.Walk, new WalkState(this));
        _stateMachine.AddState(State.Run, new RunState(this));
        _stateMachine.AddState(State.Crounch, new CrouchState(this));
        _stateMachine.AddState(State.Jump, new JumpState(this));
        _stateMachine.AddState(State.Falling, new FallingState(this));
        _stateMachine.AddState(State.Climbing, new ClimbingState(this));
        _stateMachine.AddState(State.Hide, new HideState(this));
        _stateMachine.AddState(State.Detection, new DetectionState(this));
        _stateMachine.AddState(State.Battle, new BattleState(this));
        _stateMachine.AddState(State.Attack, new AttackState(this));
        _stateMachine.AddState(State.Defence, new DefenceState(this));
        _stateMachine.AddState(State.Parry, new ParryState(this));
        _stateMachine.AddState(State.Incapacitated, new IncapacitatedState(this));
        _stateMachine.AddState(State.UsingItem, new UsingItemState(this));
        _stateMachine.AddState(State.Die, new DieState(this));

        _stateMachine.InitState(State.Idle);
    }

    private void Start()
    {
        _stateMachine.OnStart();
    } 

    private void Update()
    {
        _stateMachine.OnUpdate();

        Movement();
        Rotation();
        Debug.Log(PlayerState);
    }

    private void LateUpdate()
    {
        _stateMachine.OnLateUpdate();
    }
    private void FixedUpdate()
    {
        _stateMachine.OnFixedUpdate();
    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + new Vector3(0, maxDistance * 0.5f, 0), -transform.up * maxDistance);
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;

        if (_inputVm == null)
        {
            _inputVm = new InputViewModel();
            _inputVm.PropertyChanged += OnPropertyChanged;
            _inputVm.RegisterMoveVelocity(true);
        }
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        if ( _inputVm != null )
        {
            _inputVm.RegisterMoveVelocity(false);
            _inputVm.PropertyChanged -= OnPropertyChanged;
            _inputVm = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (_inputVm == null) return;


        _inputVm.RequestMoveOnInput(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);

        //ActorLogicManager._instance.OnMoveInput(context.ReadValue<Vector2>());
    }

    private void Movement()
    {
        if(_inputVm.Move.magnitude >= 0.1f)
        {
            Vector3 _moveDir = Quaternion.Euler(0, _inputVm.TargetAngle, 0) * Vector3.forward;

            _characterController.Move(_moveDir.normalized * moveSpeed * Time.deltaTime);
        }        
    }

    private void Rotation()
    {
        if(_inputVm.Move.magnitude >= 0.1f)
        {
            Quaternion targetRotation = Quaternion.Euler(0, _inputVm.TargetAngle, 0);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 10f * Time.deltaTime);
        }
    }
}
