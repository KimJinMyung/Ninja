using Player_State.Extension;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLockOnZone : MonoBehaviour
{
    [SerializeField] private LayerMask _mask;

    [Header("시야 각도")]
    [SerializeField] private float _ViewAngle;

    private SphereCollider ZoneCollider;

    private Transform _lockOnAbleTarget;
    private Transform _lockOnTarget;

    private bool _isLockOnMode;

    private List<Collider> hitColliders = new List<Collider>();
    private Player _player;

    private  LockOnZoneViewModel _viewModel;
    public LockOnZoneViewModel ViewModel {  get { return _viewModel; } }

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");

    PlayerInput _playerInput;
    InputActionMap actionMap;
    InputAction lockOnAction;

    private void Awake()
    {
        _player = transform.root.GetComponent<Player>();
        ZoneCollider = GetComponent<SphereCollider>();
    }

    private void OnEnable()
    {
        _mask = (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10);
        layermask = LayerMask.GetMask("Monster", "Map_Gimmixk", "Item", "LoxkOnAble", "LockOnTarget");
        _ViewAngle = 50f;

        _playerInput = transform.root.GetComponent<PlayerInput>();
        actionMap = _playerInput.actions.FindActionMap("Player");
        lockOnAction = actionMap.FindAction("LockOn");
        lockOnAction.performed += OnLockOnMode;
    }

    private void OnDisable()
    {
        lockOnAction.performed -= OnLockOnMode;
    }

    private void Start()
    {
        InitViewMondel();
    }

    private void InitViewMondel()
    {
        if (_viewModel == null)
        {
            _viewModel = new LockOnZoneViewModel();
            _viewModel.PropertyChanged += OnPropertyChanged;
            _viewModel.ReigsterLockOnTargetListChanged(true);
            _viewModel.ReigsterLockOnAbleTargetChanged(true);
            _viewModel.ReigsterLockOnTargetChanged(true);
        }
    }

    private void OnDestroy()
    {
        if (_viewModel != null)
        {
            _viewModel.ReigsterLockOnTargetChanged(false);
            _viewModel.ReigsterLockOnAbleTargetChanged(false);
            _viewModel.ReigsterLockOnTargetListChanged(false);
            _viewModel.PropertyChanged -= OnPropertyChanged;
            _viewModel = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    { 
        switch(e.PropertyName)
        {
            case nameof(_viewModel.HitColliders):
                break;
            case nameof(_viewModel.LockOnTarget):
                if(_viewModel.LockOnTarget != null)
                    _viewModel.LockOnTarget.gameObject.layer = LayerMask.NameToLayer("LockOnTarget");
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if((_mask.value & (1 << other.gameObject.layer)) != 0)
        {
            hitColliders.Add(other);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_mask.value & (1 << other.gameObject.layer)) != 0)
        {
            hitColliders.Remove(other);   
            
            if(_player.InputVm.LockOnTarget == other.transform)
            {
                _player.InputVm.RequestLockOnTarget(null);
            }
        }
    }

    private void FixedUpdate()
    {
        _lockOnAbleTarget = DetectingLookOnTarget();

        if (_lockOnAbleTarget == null) return;
        if (_lockOnAbleTarget.CompareTag("Dead")) return;
        if (_lockOnAbleTarget.gameObject.layer == LayerMask.NameToLayer("Monster"))
            _viewModel.RequestLockOnAbleTarget(_lockOnAbleTarget);
    }

    public void OnLockOnMode(InputAction.CallbackContext context)
    {
        if (hitColliders.Count <= 0) return;

        if (context.performed)
        {
            //_lockOnAbleTarget = DetectingLookOnTarget();

            if (_isLockOnMode && _lockOnAbleTarget == _lockOnTarget)
            {
                _isLockOnMode = false;
                _viewModel.RequestLockOnTarget(null, _player.InputVm);
            }
            else
            {
                _isLockOnMode = true;
                _lockOnTarget = _lockOnAbleTarget;
                _viewModel.RequestLockOnTarget(_lockOnTarget, _player.InputVm);
            }

            
        }
    }

    private LayerMask layermask;
    
    #region
    private Transform DetectingLookOnTarget()
    {
        Transform closestTarget = null;
        float closestAngle = Mathf.Infinity;

        List<Transform> tempLockOnAbleList = new List<Transform>();

        foreach(var collider in hitColliders)
        {
            Vector3 dirTarget = (collider.transform.position - Camera.main.transform.position).normalized;
            float angleToTarget = Vector3.Angle(Camera.main.transform.forward, dirTarget);

            float distance;
            float combinedMetric;

            if (angleToTarget < _ViewAngle)
            {
                distance = Vector3.Distance(Camera.main.transform.position, collider.transform.position);
                combinedMetric = angleToTarget + distance * 0.1f; // 각도와 거리를 결합한 메트릭

                if(Physics.Raycast(Camera.main.transform.position, dirTarget, out RaycastHit hit, ZoneCollider.radius))
                {
                    if(hit.collider == collider)
                    {
                        if (hit.transform.CompareTag("Dead")) continue;

                        tempLockOnAbleList.Add(collider.transform);

                        if (combinedMetric < closestAngle)
                        {
                            closestAngle = combinedMetric;
                            closestTarget = collider.transform;
                        }
                    }
                }
                
            }            
        }

        _viewModel.RequestLockOnTargetList(tempLockOnAbleList);

        if (closestTarget != null)
        {
            return closestTarget;
        }
        else
        {
            return default;
        }
    }
    #endregion
}
