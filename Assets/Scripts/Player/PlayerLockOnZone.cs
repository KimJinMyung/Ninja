using Player_State.Extension;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLockOnZone : MonoBehaviour
{
    [SerializeField] private LayerMask _mask;

    [Header("�þ� ����")]
    [SerializeField] private float _ViewAngle;

    private Transform _lockOnAbleTarget;
    private Transform _lockOnTarget;

    private bool _isLockOnMode;

    private List<Collider> hitColliders = new List<Collider>();
    private Player _player;

    private  LockOnZoneViewModel _viewModel;
    public LockOnZoneViewModel ViewModel {  get { return _viewModel; } }

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");

    private void Awake()
    {
        _player = transform.root.GetComponent<Player>();
    }

    private void OnEnable()
    {
        _mask = (1 << 6) | (1 << 7) | (1 << 8) | (1 << 9) | (1 << 10);
        _ViewAngle = 50f;
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
        if (_lockOnAbleTarget.gameObject.layer == LayerMask.NameToLayer("Monster"))
            _viewModel.RequestLockOnAbleTarget(_lockOnAbleTarget);
    }

    public void OnLockOnMode(InputAction.CallbackContext context)
    {
        if (_player.InputVm == null) return;

        if (context.started)
        {
            //_lockOnAbleTarget = DetectingLookOnTarget();

            if(_isLockOnMode && _lockOnAbleTarget == _lockOnTarget)
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
                tempLockOnAbleList.Add(collider.transform);

                distance = Vector3.Distance(Camera.main.transform.position, collider.transform.position);
                combinedMetric = angleToTarget + distance * 0.1f; // ������ �Ÿ��� ������ ��Ʈ��

                if (combinedMetric < closestAngle)
                {
                    closestAngle = combinedMetric;
                    closestTarget = collider.transform;
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