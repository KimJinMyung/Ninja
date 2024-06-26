using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

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
            _viewModel.RegisterLockOnTargetChanged(true);
        }
    }

    private void OnDisable()
    {
        if (_viewModel != null)
        {
            _viewModel.RegisterLockOnTargetChanged(false);
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
                if (_viewModel.LockOnTarget != null)
                    _viewModel.LockOnTarget.gameObject.layer = LayerMask.NameToLayer("LockOnTarget");
                break;
        }
    }

    private void FixedUpdate()
    {
        Debug.Log(DetectingTarget());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Eye.position, _viewRange);
    }


    private Transform DetectingTarget()
    {
        Transform closestTarget = null;
        float closestAngle = Mathf.Infinity;

        List<Transform> tempLockOnAbleList = new List<Transform>();

        Collider[] colliders = Physics.OverlapSphere(Eye.position, _viewRange, _lockOnAbleMask);
        if (colliders.Length <= 0) return null;

        foreach(var collider in colliders) 
        {
            Vector3 dirTarget = (collider.transform.position - Camera.main.transform.position).normalized;
            float angleToTarget = Vector3.Angle(Camera.main.transform.forward, dirTarget);

            float distance;
            float combinedMetric;

            if (angleToTarget < _viewAngle)
            {
                distance = Vector3.Distance(Camera.main.transform.position, collider.transform.position);
                combinedMetric = angleToTarget + distance * 0.1f; // 각도와 거리를 결합한 메트릭

                if (Physics.Raycast(Camera.main.transform.position, dirTarget, out RaycastHit hit, _viewRange))
                {
                    if (hit.collider == collider)
                    {
                        if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Dead")) continue;

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

        return closestTarget;
    }
}
