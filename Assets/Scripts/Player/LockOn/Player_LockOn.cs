using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_LockOn : MonoBehaviour
{
    [Header("LockOnAble Layer")]
    [SerializeField] private LayerMask _lockOnAbleMask;

    [Header("눈")]
    [SerializeField] private Transform Eye;

    [Header("감지 길이")]
    [SerializeField] private float _viewRange;

    [Header("시야 각도")]
    [SerializeField] private float _viewAngle;

    private bool isLockOnMode;

    private Player owner;

    private LockOnViewModel _viewModel;
    public LockOnViewModel ViewModel { get { return _viewModel; } }

    private Transform _lockOnAbleObject;

    private void Awake()
    {
        owner = GetComponent<Player>();
    }

    private void OnEnable()
    {
        if(_viewModel == null)
        {
            _viewModel = new LockOnViewModel();
            _viewModel.PropertyChanged += OnPropertyChanged;
            _viewModel.RegisterLockOnTargetListChanged(true);
            _viewModel.RegisterLockOnAbleTargetChanged(true);
            _viewModel.RegisterLockOnViewModel_TargetChanged(true);
        }
    }

    private void OnDisable()
    {
        if (_viewModel != null)
        {
            _viewModel.RegisterLockOnViewModel_TargetChanged(false);
            _viewModel.RegisterLockOnAbleTargetChanged(false);
            _viewModel.RegisterLockOnTargetListChanged(false);
            _viewModel.PropertyChanged -= OnPropertyChanged;
            _viewModel = null;
        }
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case nameof(_viewModel.HitColliders):
                break;
            case nameof(_viewModel.LockOnTarget):
                isLockOnMode = (_viewModel.LockOnTarget != null);
                owner.Animator.SetBool("LockOn", isLockOnMode);
                break;
        }
    }

    private void Update()
    {
        if (owner.ViewModel.LockOnTarget == null || owner.ViewModel.LockOnTarget.gameObject.layer == LayerMask.NameToLayer("Dead"))
        {
            owner.ViewModel.RequestLockOnTarget(null);
        }

        _lockOnAbleObject = DetectingTarget();
        _viewModel.RequestLockOnAbleTarget(_lockOnAbleObject);        
    }

    public void OnLockOnMode(InputAction.CallbackContext context)
    {
        if (_viewModel.HitColliders.Count <= 0) return;

        if (context.performed)
        {
            if (isLockOnMode && _lockOnAbleObject == _viewModel.LockOnTarget)
            {                
                _viewModel.RequestLockOnViewModel_Target(null, owner);
            }
            else
            {
                _viewModel.RequestLockOnViewModel_Target(_lockOnAbleObject, owner);
            }          
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Eye.position, _viewRange);
    }

    private Transform DetectingTarget()
    {
        Collider[] colliders = Physics.OverlapSphere(Eye.position, _viewRange, _lockOnAbleMask);

        return ReturnTarget(colliders);
    }

    Transform ReturnTarget(Collider[] colliders)
    {
        Transform closestTarget = null;
        float closestAngle = Mathf.Infinity;

        List<Transform> tempLockOnAbleList = new List<Transform>();

        foreach (var collider in colliders)
        {
            Vector3 dirTarget = (collider.transform.position - Camera.main.transform.position).normalized;
            float angleToTarget = Vector3.Angle(Camera.main.transform.forward, dirTarget);

            if (angleToTarget < _viewAngle)
            {
                float cameraDis = Vector3.Distance(new Vector3(Camera.main.transform.position.x, 0, Camera.main.transform.position.z), new Vector3(transform.position.x, 0, transform.position.z));
                if (Physics.Raycast(Camera.main.transform.position, dirTarget, out RaycastHit hit, _viewRange + cameraDis, _lockOnAbleMask))
                {
                    if (hit.collider == collider)
                    {                       
                        tempLockOnAbleList.Add(collider.transform);

                        if (angleToTarget < closestAngle)
                        {
                            closestAngle = angleToTarget;
                            closestTarget = hit.transform;
                        }
                    }
                }

            }
        }

        Transform _lockOnTarget = owner.ViewModel.LockOnTarget;

        if (_lockOnTarget != null && !tempLockOnAbleList.Contains(_lockOnTarget) && _lockOnTarget.gameObject.layer != LayerMask.NameToLayer("Dead")) tempLockOnAbleList.Add(_lockOnTarget);

        _viewModel.RequestLockOnTargetList(tempLockOnAbleList);

        return closestTarget;
    }
}
