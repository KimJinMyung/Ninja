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
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_mask.value & (1 << other.gameObject.layer)) != 0)
        {
            hitColliders.Remove(other);
            MonsterManager.instance.LockOnAbleListRemove(other.transform);
        }
    }

    private void FixedUpdate()
    {
        DetectingLookOnTarget();

        if (_isLockOnMode)
        {
            _player.InputVm.RequestLockOnTarget(_lockOnTarget);
        }
        else
        {
            _player.InputVm.RequestLockOnTarget(null);
        }
    }

    public void OnLockOnMode(InputAction.CallbackContext context)
    {
        if (_player.InputVm == null) return;

        if (context.started)
        {
            Transform newTarget = DetectingLookOnTarget();
            Debug.Log(newTarget.GetInstanceID());
            if (_lockOnTarget == newTarget)
            {
                _lockOnTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
                _isLockOnMode = false;
            }
            else _isLockOnMode = true;

            if(_lockOnTarget != null)
                _lockOnTarget.gameObject.layer = LayerMask.NameToLayer("Monster");
            _lockOnTarget = newTarget;

            _player.Animator.SetBool(hashLockOn, _isLockOnMode);

            if (!_isLockOnMode)
            {
                _lockOnTarget = null;
            }
;
        }
    }

    private Transform DetectingLookOnTarget()
    {
        Transform closestTarget = null;
        float closestAngle = Mathf.Infinity;

        foreach(var collider in hitColliders)
        {
            Vector3 dirTarget = (collider.transform.position - Camera.main.transform.position).normalized;
            float angleToTarget = Vector3.Angle(Camera.main.transform.forward, dirTarget);

            float distance;
            float combinedMetric;

            if (angleToTarget < _ViewAngle)
            {
                MonsterManager.instance.LockOnAbleListAdd(collider.transform);
                if (_lockOnTarget == closestTarget && _lockOnTarget != null) continue;

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
                MonsterManager.instance.LockOnAbleListRemove(collider.transform);
            }
        }

        if (closestTarget != null)
        {
            return closestTarget;
        }
        else
        {
            return default;
        }
    }
}
