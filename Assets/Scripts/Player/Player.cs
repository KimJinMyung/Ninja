using ActorStateMachine;
using System.ComponentModel;
using UnityEngine;
using System.Collections;
using UnityEngine.InputSystem;
using System.Buffers;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;

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
    public float _velocity {  get; set; }

    public bool isGravityAble { get; set; } = true;

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

    [Header("Stamina Recovery")]
    [SerializeField] private float StaminaRecoveryDelayTime;
    private float _staminaRecoveryDelaytimer;

    [HideInInspector]
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
        _viewModel.ReigsterAssassinatedTypeChanged(true);
        _viewModel.BindPlayerMaxHPChangedEvent(true);
        _viewModel.BindPlayerHPChangedEvent(true);
        _viewModel.BindPlayerMaxStaminaChangedEvent(true);
        _viewModel.BindPlayerStaminaChangedEvent(true);
        _viewModel.BindPlayerLifeCountChangedEvent(true);

        SetPlayerInfo();
        InitRotation();

        _viewModel.RequestStateChanged(player_id, State.Idle);

        _staminaRecoveryDelaytimer = 0f;
    }

    private void OnDisable()
    {
        if (_viewModel != null)
        {
            _viewModel.BindPlayerLifeCountChangedEvent(false);
            _viewModel.BindPlayerMaxStaminaChangedEvent(false);
            _viewModel.BindPlayerStaminaChangedEvent(false);
            _viewModel.BindPlayerMaxHPChangedEvent(false);
            _viewModel.BindPlayerHPChangedEvent(false);
            _viewModel.ReigsterAssassinatedTypeChanged(false);
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

    public void OnResurrection(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            if(_viewModel.playerState == State.Die && isResurrectionAble)
            {
                ResurrectPlayer();
                return;
            }
        }
    }

    public void ResurrectPlayer()
    {
        _viewModel.RequestStateChanged(player_id, State.Idle);
        _viewModel.playerInfo.HP = _viewModel.playerInfo.MaxHP;
        _viewModel.playerInfo.Stamina = _viewModel.playerInfo.MaxStamina;
        isResurrectionAble = false;
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

        IsDead();

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
        RecoveryStamina();

        _stateMachine.OnUpdate();
    }

    private void FixedUpdate()
    {
        _stateMachine.OnFixedUpdate();

        Rotation();
    }

    public bool isResurrectionAble;

    private void IsDead()
    {
        if(_viewModel.playerState == State.Die)
        {
            if(_viewModel.playerInfo.Life <= 0)
            {
                //사망 UI On
                //isResurrectionAble = true;
            }
        }
    }

    private void MoveSpeed()
    {
        if (_viewModel.playerInfo == null) return;

        if (_isRun)
        {
            movementSpeed = _viewModel.playerInfo.RunSpeed;
        }
        else
        {
            movementSpeed = _viewModel.playerInfo.WalkSpeed;
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
        if (!isGravityAble) return;

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

        if (IsDefenceSuccess(attacker.transform.position))
        {
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
                if(attacker.Type == MonsterType.Boss && (attacker.BossAttackTypeIndex == 0 || attacker.BossAttackTypeIndex == 2))
                {
                    StartCoroutine(PushBack(attacker.transform.position, 3));
                }

                //방어 성공
                Player_data StaminaData = _viewModel.playerInfo;
                StaminaData.Stamina -= attacker.MonsterViewModel.MonsterInfo.Strength;
                _viewModel.RequestPlayerDataChanged(player_id, StaminaData);

                if (_viewModel.playerInfo.Stamina > 0f)
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

        Player_data HPData = _viewModel.playerInfo;
        HPData.HP -= damage;
        _viewModel.RequestPlayerDataChanged(player_id, HPData);

        //UnityEngine.Debug.Log(player_info.HP);
        AttackDir(attacker.transform.position);

        if (_viewModel.playerInfo.HP > 0f)
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

    IEnumerator PushBack(Vector3 attackerPosition, float AddPower)
    {
        Vector3 dir = transform.position - attackerPosition;
        dir.y = 0;
        dir.Normalize();

        float timer = 0f;

        while(timer < 1f)
        {
            playerController.Move(dir * AddPower * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        yield break;
    }

    private void AttackDir(Vector3 attakerPosition)
    {
        StartCoroutine(PushBack(attakerPosition, 5));

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

    private void RecoveryStamina()
    {
        if (StaminaRecoveryDelayTime <= _staminaRecoveryDelaytimer)
        {
            Player_data StaminaData = _viewModel.playerInfo;
            StaminaData.Stamina = Mathf.Clamp(StaminaData.Stamina + Time.deltaTime, 0, _viewModel.playerInfo.MaxStamina);
            _viewModel.RequestPlayerDataChanged(player_id, StaminaData);
            return;
        }

        if (_viewModel.playerInfo.Stamina < _viewModel.playerInfo.MaxStamina && (_viewModel.playerState == State.Battle || _viewModel.playerState == State.Idle)) _staminaRecoveryDelaytimer += Time.deltaTime;
        else _staminaRecoveryDelaytimer = 0f;
    }
}
