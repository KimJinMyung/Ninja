using ActorStateMachine;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using UnityEngine;

using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    CharacterController playerController;    

    Vector3 moveDir;

    float MoveAngle;

    #region gravity
    //중력 값
    private float gravity = -9.81f;
    //현재 중력 가속도
    private float _velocity;

    [Header("그라운드 확인 overlap")]
    [SerializeField]
    private Transform overlapPos;

    [SerializeField]
    private LayerMask gravityLayermask;
    #endregion

    [Header("Jump")]
    [SerializeField] private float JumpForce;

    private Player_data player_info;
    public int player_id {  get; private set; }

    [Header("Player Cinemachine Setting")]
    public Cinemachine.AxisState x_Axis;
    public Cinemachine.AxisState y_Axis;

    [SerializeField] private Transform _lookAt;

    private Quaternion mouseRotation;
    private Quaternion initRotation;

    private Animator animator;
    public Animator Animator { get { return animator; } set { animator = value; } }

    private bool isGround;
    public bool _isRun {  get; private set; }
    private float movementSpeed;

    #region ViewModel
    private Player_ViewModel _viewModel;
    public Player_ViewModel ViewModel { get { return _viewModel; } }
    #endregion
    private StateMachine _stateMachine;

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();

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

        if (_viewModel == null)
        {
            _viewModel = new Player_ViewModel();
        }
    }

    private void OnEnable()
    {
        player_id = GetInstanceID();

        _viewModel.PropertyChanged += OnPropertyChanged;
        _viewModel.RegisterStateChanged(player_id, true);
        _viewModel.RegisterMoveVelocity(true);
        _viewModel.ReigsterLockOnTargetChanged(true);

        SetPlayerInfo();
        InitRotation();        
    }

    private void OnDisable()
    {
        if (_viewModel != null)
        {
            _viewModel.ReigsterLockOnTargetChanged(false);
            _viewModel.RegisterMoveVelocity(false);
            _viewModel.RegisterStateChanged(player_id, false);
            _viewModel.PropertyChanged -= OnPropertyChanged;
            _viewModel = null;
        }
    }
    private void SetPlayerInfo()
    {
        var player = DataManager.Instance.GetPlayerData(0);
        if (player == null) return;

        player_info = player;
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (_viewModel == null) return;

        _viewModel.RequestMoveOnInput(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_viewModel == null) return;

        if (isGround)
        {
            _velocity = Mathf.Sqrt(JumpForce * -1f * gravity);
            UnityEngine.Debug.Log("점프");
        }
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (_viewModel == null) return;

        _isRun = context.ReadValue<float>() > 0.5f;
    }

    private void Update()
    {
        Gravity();
        MoveSpeed();

        CameraRotation();
        Movement();
        
        _stateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {       
        _stateMachine.OnFixedUpdate();

        Rotation();
    }       

    private void MoveSpeed()
    {
        if(player_info == null) return;

        if (_isRun)
        {
            movementSpeed = player_info.RunSpeed;
        }
        else
        {
            movementSpeed = player_info.WalkSpeed;
        }
    }

    private void Movement()
    {
        if (!animator.GetBool(hashIsMoveAble)) return;
        moveDir = new Vector3(_viewModel.Move.x, 0, _viewModel.Move.y).normalized;

        if(moveDir.magnitude >= 0.1f)
        {
            MoveAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
            Vector3 dir = Quaternion.Euler(0, MoveAngle, 0) * Vector3.forward;            

            playerController.Move(dir.normalized * movementSpeed * Time.deltaTime);
        }
    }
    private void Gravity()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f, gravityLayermask) && _velocity <= 0.1f)
        {
            _velocity = 0f;
            isGround = true;
        }
        else
        {
            _velocity += gravity * 0.5f * Time.deltaTime;
            isGround = false;
        }

        playerController.Move(new Vector3(0, _velocity, 0) * Time.deltaTime);
    }

    private void Rotation()
    {
        if (!animator.GetBool(hashIsMoveAble)) return;

        if (animator.GetBool(hashLockOn))
        {
            LookAtTargetOnYAxis(_viewModel.LockOnTarget, transform);
        }
        else
        {
            CharacterMeshRotation(transform);
        }
    }

    private void LookAtTargetOnYAxis(Transform target, Transform playerMesh)
    {
        if(target == null) return;

        Vector3 dirTarget = target.position - playerMesh.position;
        dirTarget.y = 0;
        Quaternion rotation = Quaternion.LookRotation(dirTarget);
        playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, Quaternion.Euler(0, rotation.eulerAngles.y, 0), 10f * Time.fixedDeltaTime);
    }

    private void CharacterMeshRotation(Transform playerMesh)
    {
        if (ViewModel.Move.magnitude >= 0.1f)
        {
            Quaternion cameraDir = Quaternion.Euler(0, MoveAngle, 0);

            Quaternion targetRotate = Quaternion.Lerp(transform.rotation, cameraDir, 100f * Time.fixedDeltaTime);

            ViewModel.RequestActorRotate(targetRotate.x, targetRotate.y, targetRotate.z);

            //Quaternion playerRotation = Quaternion.Lerp(playerMesh.rotation, targetRotate, 10f * Time.deltaTime);
            playerMesh.rotation = Quaternion.Lerp(playerMesh.rotation, targetRotate, 10f * Time.fixedDeltaTime);
        }
    }

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

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
        switch (e.PropertyName)
        {
            case nameof(ViewModel.playerState):
                _stateMachine.ChangeState(ViewModel.playerState);
                break;
        }
    }
}
