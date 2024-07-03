using ActorStateMachine;
using System.ComponentModel;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Buffers;
using System.Collections.Generic;

public class Player : MonoBehaviour
{
    [Header("Katana Mesh")]
    [SerializeField] private GameObject EquipedKatana;
    public GameObject Katana { get {  return EquipedKatana; } }
    [SerializeField] private GameObject UnEquipedKatana;
    public GameObject KatanaCover { get { return UnEquipedKatana; } }

    public CharacterController playerController {  get; private set; }

    Vector3 moveDir;

    float MoveAngle;

    #region gravity
    //중력 값
    private float gravity = -20;
    public float GravityValue { get { return gravity; } }
    //현재 중력 가속도
    public float _velocity { get; private set; }

    [Header("그라운드 확인 overlap")]
    [SerializeField]
    private Transform overlapPos;

    [SerializeField]
    private LayerMask gravityLayermask;
    #endregion

    [Header("Jump")]
    [SerializeField] private float JumpForce;

    [Header("Climb")]
    [SerializeField] List<ParkourAction> parkourActions;

    private Player_data player_info;
    public Player_data Player_Info { get { return player_info; } }
    public int player_id {  get; private set; }

    [Header("Player Cinemachine Setting")]
    public Cinemachine.AxisState x_Axis;
    public Cinemachine.AxisState y_Axis;

    [SerializeField] private Transform _lookAt;

    private Quaternion mouseRotation;
    private Quaternion initRotation;

    private Animator animator;
    public Animator Animator { get { return animator; } set { animator = value; } }

    public bool isGround { get; private set; }
    public bool _isRun {  get; private set; }
    private float movementSpeed;

    #region ViewModel
    private Player_ViewModel _viewModel;
    public Player_ViewModel ViewModel { get { return _viewModel; } }
    #endregion
    private StateMachine _stateMachine;
    private EnvironmentScanner _environmentScanner;

    public bool isDefence;

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");
    protected readonly int hashIsMoveAble = Animator.StringToHash("IsMoveAble");
    protected readonly int hashDefence = Animator.StringToHash("Defence");
    protected readonly int hashHurtDir_z = Animator.StringToHash("HurtDir_z");
    protected readonly int hashHurtDir_x = Animator.StringToHash("HurtDir_x");
    protected readonly int hashHurt = Animator.StringToHash("Hurt");

    private void Awake()
    {
        animator = GetComponent<Animator>();
        playerController = GetComponent<CharacterController>();
        _environmentScanner = GetComponent<EnvironmentScanner>();

        _stateMachine = gameObject.AddComponent<StateMachine>();

        AddState();

        _stateMachine.InitState(State.Idle);

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        if (_viewModel == null)
        {
            _viewModel = new Player_ViewModel();
        }
    }

    private void AddState()
    {
        _stateMachine.AddState(State.Idle, new Player_IdleState(this));
        _stateMachine.AddState(State.Crounch, new CrouchState(this));
        _stateMachine.AddState(State.Jump, new JumpState(this));
        _stateMachine.AddState(State.Falling, new FallingState(this));
        _stateMachine.AddState(State.Climbing, new ClimbingState(this));
        _stateMachine.AddState(State.Hide, new HideState(this));
        _stateMachine.AddState(State.Detection, new DetectionState(this));
        _stateMachine.AddState(State.Battle, new BattleState(this));
        _stateMachine.AddState(State.Attack, new AttackState(this));
        _stateMachine.AddState(State.Assasinate, new AssassinatedState(this));
        _stateMachine.AddState(State.Defence, new DefenceState(this));
        _stateMachine.AddState(State.Parry, new ParryState(this));
        _stateMachine.AddState(State.Incapacitated, new IncapacitatedState(this));
        _stateMachine.AddState(State.UsingItem, new UsingItemState(this));
        _stateMachine.AddState(State.Grappling , new GrapplingState(this));
        _stateMachine.AddState(State.Hurt, new HurtState(this));
        _stateMachine.AddState(State.Die, new DieState(this));
    }

    private void OnEnable()
    {
        player_id = GetInstanceID();

        _viewModel.PropertyChanged += OnPropertyChanged;
        _viewModel.RegisterStateChanged(player_id, true);
        _viewModel.RegisterPlayerDataChanged(player_id, true);
        _viewModel.RegisterMoveVelocity(true);
        _viewModel.ReigsterLockOnTargetChanged(true);

        SetPlayerInfo();
        InitRotation();

        _viewModel.RequestStateChanged(player_id, State.Idle);
    }

    private void OnDisable()
    {
        if (_viewModel != null)
        {
            _viewModel.ReigsterLockOnTargetChanged(false);
            _viewModel.RegisterMoveVelocity(false);
            _viewModel.RegisterPlayerDataChanged(player_id, false);
            _viewModel.RegisterStateChanged(player_id, false);
            _viewModel.PropertyChanged -= OnPropertyChanged;
            _viewModel = null;
        }
    }
    private void SetPlayerInfo()
    {
        var player = DataManager.Instance.GetPlayerData(0);
        if (player == null) return;

        player_info = player.Clone();
        _viewModel.RequestPlayerDataChanged(player_id, player.Clone());
    }

    public void OnMovement(InputAction.CallbackContext context)
    {
        if (_viewModel == null) return;

        _viewModel.RequestMoveOnInput(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y);
    }

    public Vector3 ClimbingPos { get; private set; }
    public ParkourAction currentAction { get; private set; }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (_viewModel == null) return;
        if(!(_viewModel.playerState == State.Idle || _viewModel.playerState == State.Battle)) return;

        if (context.performed)
        {
            if (isGround && _viewModel.playerState != State.Climbing)
            {
                var hitData = _environmentScanner.ObstacleCheck();
                if (hitData.forwardHitFound)
                {
                    ClimbingPos = hitData.heightHit.point;

                    foreach (var action in parkourActions)
                    {
                        if (action.CheckIfPossible(hitData, this.transform))
                        {
                            ClimbAnimationStart(action);
                            return;
                        }
                    }

                }

                _velocity = Mathf.Sqrt(JumpForce * -1f * gravity);
                Debug.Log("점프");

            }            
        }        
    }    

    private void ClimbAnimationStart(ParkourAction action)
    {
        currentAction = action;

        animator.SetInteger("Climb_Value", (int)action.Climb_Value);
        animator.SetBool("Climbing", true);
        animator.SetTrigger("Climb");
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

        if (!animator.GetBool(hashLockOn))
        {
            CameraRotation_Move();
        }
        else
        {
            CamearaRotation_Target(_viewModel.LockOnTarget);
        }

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
        if (_viewModel.playerState == State.Climbing) return;

        Debug.DrawRay(transform.position, Vector3.down * 0.01f, Color.red);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 0.01f, gravityLayermask) && _velocity <= 0.1f)
        {
            _velocity = 0f;
            isGround = true;
        }
        else if (_viewModel.playerState == State.Grappling)
        {
            _velocity += gravity * 0.75f * Time.deltaTime;
            isGround = false;
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

    private void CamearaRotation_Target(Transform target)
    {
        if (target == null) return;

        // target을 향한 회전 값을 계산
        Quaternion targetRotation = Quaternion.LookRotation(target.position - _lookAt.position);

        // 부드러운 회전을 위해 Slerp 사용
        _lookAt.rotation = Quaternion.Slerp(_lookAt.rotation, targetRotation, Time.deltaTime * 10f);
        InitRotation();
    }

    private void CameraRotation_Move()
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
            case nameof(ViewModel.LockOnTarget):
                if (ViewModel.LockOnTarget != null) animator.SetBool(hashLockOn, true);
                else animator.SetBool(hashLockOn, false);
                break;
        }
    }

    public void Hurt(Monster attacker, float damage)
    {
        if (_viewModel.playerState == State.Die) return;
        if (_viewModel.playerState == State.Assasinate) return;

        if (_viewModel.playerState == State.Parry)
        {
            if (attacker.MonsterViewModel.CurrentAttackMethod.AttackType != "Long")
            {
                attacker.Parried(this);
                return;
            }
            //원거리 공격은 아무런 효과가 없음으로 처리
            return;
        }

        if (animator.GetBool(hashDefence))
        {
            if (IsDefenceSuccess(attacker.transform.position))
            {
                //방어 성공
                player_info.Stamina -= attacker.MonsterViewModel.MonsterInfo.Strength;
                if(player_info.Stamina > 0f)
                {
                    _viewModel.RequestStateChanged(player_id, State.Defence);
                    return;
                }
                else
                {
                    _viewModel.RequestStateChanged(player_id, State.Incapacitated);
                    return;
                }
            }
        }

        player_info.HP -= damage;

        //UnityEngine.Debug.Log(player_info.HP);
        AttackDir(attacker.transform.position);

        if (player_info.HP > 0f)
        {
            _viewModel.RequestStateChanged(player_id, State.Hurt);
            return;
        }
        else
        {
            _viewModel.RequestStateChanged(player_id, State.Die);
            return;
        }
    }

    private void AttackDir(Vector3 attakerPosition)
    {
        Vector3 knockbackDir = transform.position - attakerPosition;
        knockbackDir.y = 0;
        knockbackDir.Normalize();

        animator.SetFloat(hashHurtDir_z, knockbackDir.z);
        animator.SetFloat(hashHurtDir_x, knockbackDir.x);
    }

    private bool IsDefenceSuccess(Vector3 attackerPosition)
    {
        Vector3 attackDir = attackerPosition - transform.position;
        attackDir.y = 0;

        Vector3 playerForward = transform.forward;
        playerForward.y = 0;

        float angle = Vector3.Angle(playerForward, attackDir);
        if(angle <= 45f) return true;
        else return false;
    }
}
