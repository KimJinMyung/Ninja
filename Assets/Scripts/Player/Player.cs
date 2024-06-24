using Cinemachine;
using Cinemachine.Utility;
using Player_State.Extension;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using ActorStateMachine;
using TMPro;

public class Player : MonoBehaviour
{
    private int _playerId;
    public int PlayerId { get { return _playerId; } }

    //Ground 확인
    [Header("Check IsGround")]
    [SerializeField] private float maxDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform _lookAt;

    [Header("Player Cinemachine Setting")]
    public Cinemachine.AxisState x_Axis;
    public Cinemachine.AxisState y_Axis;

    private Player_data player_Data;

    //Movement
    private Vector3 _inputMoveDir;
    private float targetAngle;

    public bool _isRun {  get; private set; }

    #region ViewModel
    private InputViewModel _inputVm;

    public InputViewModel InputVm { get { return _inputVm; } }

    #endregion

    [Header("Character Controller")]
    [SerializeField] private CharacterController _characterController;

    [Header("Player Animation")]
    [SerializeField] private Animator _animator;
    public Animator Animator { get { return _animator; } }

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    protected readonly int hashParry = Animator.StringToHash("Parry");

    [SerializeField] private float _attackDelay;
    public float AttackDelay { get { return _attackDelay; } }

    private float moveSpeed;

    #region isGround
    public float MaxDistance { get { return maxDistance; } }

    public LayerMask GroundLayer { get { return groundLayer; } }
    #endregion

    [Header("Debug Mode")]
    [SerializeField] private bool _debug;

    public bool _Debug { get { return _debug; } }

    private StateMachine _stateMachine;

    public StateMachine StateMachine { get { return _stateMachine; } }

    public Vector3 Velocity { get { return _characterController.velocity; } } 
    private Quaternion initRotation;
    public Vector3 _moveDir { get; private set; }

    private void Awake()
    {
        _playerId = GetInstanceID();

        _stateMachine = gameObject.AddComponent<StateMachine>();

        _stateMachine.AddState(State.Idle, new Player_IdleState(this));
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
        _stateMachine.AddState(State.Hurt, new HurtState(this));
        _stateMachine.AddState(State.Die, new DieState(this));

        _stateMachine.InitState(State.Idle);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
    }

    private void Start()
    {
        _stateMachine.OnStart();

        if (_inputVm == null)
        {
            _inputVm = new InputViewModel();
            _inputVm.PropertyChanged += OnPropertyChanged;
            _inputVm.RegisterStateChanged(_playerId, true);
            _inputVm.RegisterMoveVelocity(true);
            _inputVm.RegisterActorRotate(true);
            _inputVm.ReigsterLockOnTargetChanged(true);
            _inputVm.ReigsterPlayerInfoChanged(_playerId, true);
        }

        SetPlayerInfo();
        moveSpeed = player_Data.WalkSpeed;
    } 

    private void SetPlayerInfo()
    {
        var player = DataManager.Instance.GetPlayerData(0);
        if (player == null) return;

        player_Data = player;
        _inputVm.RequestOnPlayerInfo(PlayerId,player_Data);
    }

    private void Update()
    {
        UpdatePosition();
        
        CameraRotation();

        Movement();

        _stateMachine.OnUpdate();
    }

    private void LateUpdate()
    {
        _stateMachine.OnLateUpdate();
    }
    private void FixedUpdate()
    {
        _stateMachine.OnFixedUpdate();

        Rotation();

        //_lockOnTarget = DetectingLockOnTarget();


    }

    private void OnDrawGizmos()
    {
        if (!_debug) return;

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + new Vector3(0, maxDistance * 0.5f, 0), -transform.up * maxDistance);
    }

    private void OnEnable()
    {
        InitRotation();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;

        if ( _inputVm != null )
        {
            _inputVm.ReigsterPlayerInfoChanged(_playerId, false);
            _inputVm.ReigsterLockOnTargetChanged(false);
            _inputVm.RegisterActorRotate(false);
            _inputVm.RegisterMoveVelocity(false);
            _inputVm.RegisterStateChanged(_playerId, false);

            _inputVm.PropertyChanged -= OnPropertyChanged;
            _inputVm = null;
        }
    }

    #region Input System Event
    public void OnMove(InputAction.CallbackContext context)
    {
        if (_inputVm == null) return;

        _inputVm.RequestMoveOnInput(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (_inputVm == null) return;

        if (context.started)
        {
            if (_inputVm.PlayerState == State.Attack) return;

            if (_inputVm.PlayerState == State.Defence) Animator.SetTrigger(hashParry);
            else
            {
                if (_inputVm.PlayerState.Equals(State.Parry)) return;
                _inputVm.RequestStateChanged(_playerId, State.Attack);
            }
        }
    }

    public void OnDefence(InputAction.CallbackContext context)
    {
        if (_inputVm == null) return;
        if (_inputVm.PlayerState == State.Attack) return;

        if (context.ReadValue<float>() > 0.5f) _inputVm.RequestStateChanged(_playerId, State.Defence);
        else _inputVm.RequestStateChanged(_playerId, State.Battle);
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (_inputVm == null) return;
        
        if(context.ReadValue<float>() > 0.5f) moveSpeed = player_Data.RunSpeed;
        else moveSpeed = player_Data.WalkSpeed;
    }
    #endregion

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_inputVm.PlayerState):
                _stateMachine.ChangeState(_inputVm.PlayerState);
                break;
            case nameof(_inputVm.LockOnTarget):
                if (_inputVm.LockOnTarget != null) Animator.SetBool(hashLockOn, true);
                else Animator.SetBool(hashLockOn, false);
                break;
            case nameof(_inputVm.player_Data):
                if(_inputVm.player_Data.HP <= 0) _inputVm.RequestStateChanged(PlayerId, State.Die);
                else _inputVm.RequestStateChanged(PlayerId, State.Hurt);
                break;
        }
    }

    private void Movement()
    {
        if (!Animator.GetBool(hashIsMoveAble)) return;

        _inputMoveDir = new Vector3(_inputVm.Move.x, 0, _inputVm.Move.y).normalized;

        if (_inputMoveDir.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(_inputMoveDir.x, _inputMoveDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;

            _moveDir = Quaternion.Euler(0, targetAngle, 0) * Vector3.forward;

            _characterController.Move(_moveDir.normalized * moveSpeed * Time.deltaTime);
        }        
    }

    private void Rotation()
    {
        if (!Animator.GetBool(hashIsMoveAble)) return;

        //캐릭터 Mesh
        Transform playerMesh = transform.GetChild(0);

        if (Animator.GetBool(hashLockOn) && _inputVm.LockOnTarget != null)
        {
            LookAtTargetOnYAxis(_inputVm.LockOnTarget, playerMesh);
        }
        else
        {
            CharacterMeshRotation(playerMesh);
        }
        
    }

    private void LookAtTargetOnYAxis(Transform target, Transform playerMesh)
    {
        Vector3 dirTarget = target.position - playerMesh.position;
        dirTarget.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dirTarget);
        playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 10f * Time.fixedDeltaTime);
    }

    private void CharacterMeshRotation(Transform playerMesh)
    {
        if (_inputVm.Move.magnitude >= 0.1f)
        {
            Quaternion cameraDir = Quaternion.Euler(0, targetAngle, 0);

            Quaternion targetRotate = Quaternion.Lerp(transform.rotation, cameraDir, 100f * Time.fixedDeltaTime);

            _inputVm.RequestActorRotate(targetRotate.x, targetRotate.y, targetRotate.z);

            //Quaternion playerRotation = Quaternion.Lerp(playerMesh.rotation, targetRotate, 10f * Time.deltaTime);
            playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, targetRotate, 10f * Time.fixedDeltaTime);
        }
    }

    Quaternion mouseRotation;

    private void InitRotation()
    {
        x_Axis.Value = 0;
        y_Axis.Value = 0;

        initRotation = _lookAt.rotation;

        Vector3 initEulerAngle = initRotation.eulerAngles;
        x_Axis.Value = initEulerAngle.y;
        y_Axis.Value = initEulerAngle.x;

        mouseRotation = initRotation;
    }

    private void CameraRotation()
    {
        x_Axis.Update(Time.fixedDeltaTime);
        y_Axis.Update(Time.fixedDeltaTime);

        mouseRotation = Quaternion.Euler(y_Axis.Value, x_Axis.Value, 0f);

        _lookAt.rotation = Quaternion.Lerp(_lookAt.rotation, mouseRotation, 1f);
    }

    private void UpdatePosition()
    {
        transform.position = _characterController.transform.position;
    }
}
