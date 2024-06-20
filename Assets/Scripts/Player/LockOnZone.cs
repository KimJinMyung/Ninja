using Player_State.Extension;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class LockOnZone : MonoBehaviour
{
    [SerializeField] private LayerMask _mask;
    [SerializeField] private float _ViewAngle;

    private Transform _lockOnTarget;
    private bool _isLockOnMode;

    private List<Collider> hitColliders = new List<Collider>();
    private Player _player;

    protected readonly int hashLockOn = Animator.StringToHash("LockOn");

    private void Awake()
    {
        _player = transform.root.GetComponent<Player>();//GetComponent<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if((_mask.value & (1 << other.gameObject.layer)) != 0)
        {
            hitColliders.Add(other);
            Debug.LogWarning("aaaa");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_mask.value & (1 << other.gameObject.layer)) != 0)
        {
            hitColliders.Remove(other);
            MonsterManager.instance.LockOnAbleListRemove(other.transform);
            Debug.LogWarning("cccc");
        }
    }

    private void FixedUpdate()
    {
        _lockOnTarget = DetectingLookOnTarget();

        if (_isLockOnMode)
        {
            _player.InputVm.RequestLockOnTarget(_lockOnTarget);
        }
        else
        {
            _player.InputVm.RequestLockOnTarget(null);

            if (_lockOnTarget != null)
                _lockOnTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
        }
    }

    public void OnLockOnMode(InputAction.CallbackContext context)
    {
        if (_player.InputVm == null) return;
        if (_lockOnTarget == null) return;

        if (context.started)
        {
            _isLockOnMode = _isLockOnMode ? false : true;
            _player.Animator.SetBool(hashLockOn, _isLockOnMode);

            if (!_isLockOnMode)
            {
                _lockOnTarget = null;
            }

            //_inputVm.RequestLockOnTarget(_lockOnTarget);
        }
    }

    private Transform DetectingLookOnTarget()
    {
        Transform closestTarget = null;
        float closestAngle = Mathf.Infinity;

        foreach(Collider collider in hitColliders)
        {
            Vector3 dirTarget = (collider.transform.position - Camera.main.transform.position).normalized;
            float angleToTarget = Vector3.Angle(Camera.main.transform.forward, dirTarget);

            float distance;
            float combinedMetric;

            if (_lockOnTarget == collider.transform)
            {
                distance = Vector3.Distance(Camera.main.transform.position, collider.transform.position);
                combinedMetric = angleToTarget + distance * 0.1f;

                closestAngle = combinedMetric;
                closestTarget = collider.transform;
            }
            else
            {
                if (angleToTarget < _ViewAngle)
                {
                    MonsterManager.instance.LockOnAbleListAdd(collider.transform);
                    if (_lockOnTarget == closestTarget) continue;

                    distance = Vector3.Distance(Camera.main.transform.position, collider.transform.position);
                    combinedMetric = angleToTarget + distance * 0.1f; // 각도와 거리를 결합한 메트릭

                    if (combinedMetric < closestAngle)
                    {
                        closestAngle = combinedMetric;
                        closestTarget = collider.transform;
                    }
                }
                else
                {
                    if (_lockOnTarget == closestTarget) continue;
                    MonsterManager.instance.LockOnAbleListRemove(collider.transform);
                }
            }
        }

        if (closestTarget != null)
        {
            Debug.Log(closestTarget.gameObject.name);

            if (closestTarget != _lockOnTarget && _lockOnTarget != null)
            {
                _lockOnTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
            }
            return closestTarget;
        }
        else
        {
            Debug.Log("감지 실패");
            return default;
        }
    }
}
