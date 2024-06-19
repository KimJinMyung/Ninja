using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;

    private Dictionary<int, Action<State>> _stateChangedCallback = new Dictionary<int, Action<State>>();
    private Action<float, float> _moveVelocityChangedCallback;
    private Action<float, float, float> _targetAngleChangedCallback;
    private Action<bool> _isLockOnModeChangedCallback;
    private Dictionary<int, Action<float>> _hpChangedCallbacks = new Dictionary<int, Action<float>>();
    private Action<Transform> _lockOnTargetChangedCallback;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if(_instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    #region Register 연결부
    public void RegisterStateChangedCallback(int actorId,Action<State> stateChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_stateChangedCallback.ContainsKey(actorId))
            {
                _stateChangedCallback[actorId] = stateChangedCallback;
            }
            else
            {
                _stateChangedCallback[actorId] += stateChangedCallback;
            }
        }
        else
        {
            if(_stateChangedCallback.ContainsKey(actorId))
            {
                _stateChangedCallback[actorId] -= stateChangedCallback;
                if (_stateChangedCallback[actorId] == null) _stateChangedCallback.Remove(actorId);
            }
        }
    }

    public void RegisterMoveVelocityChangedCallback(Action<float, float> moveVelocityChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            _moveVelocityChangedCallback += moveVelocityChangedCallback;
        }
        else
        {
            _moveVelocityChangedCallback -= moveVelocityChangedCallback;
        }
    }

    public void RegisterActorRotateChangedCallback(Action<float, float, float> targetAngleChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            _targetAngleChangedCallback += targetAngleChangedCallback;
        }
        else
        {
            _targetAngleChangedCallback -= targetAngleChangedCallback;
        }
    }

    public void RegisterHpChangedCallback(int actorId,Action<float> hpChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_hpChangedCallbacks.ContainsKey(actorId))
            {
                _hpChangedCallbacks[actorId] = hpChangedCallback;
            }
            else
            {
                _hpChangedCallbacks[actorId] += hpChangedCallback;
            }
        }
        else
        {
            if (_hpChangedCallbacks.ContainsKey(actorId))
            {
                _hpChangedCallbacks[actorId] -= hpChangedCallback;
                if (_hpChangedCallbacks[actorId] == null) _hpChangedCallbacks.Remove(actorId);
            }
        }
    }

    public void RegisterLockOnTargetChangedCallback(Action<Transform> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnTargetChangedCallback += lockOnTargetChangedCallback;
        else _lockOnTargetChangedCallback -= lockOnTargetChangedCallback;
    }

    #endregion

    #region Request 연결부
    public void OnChangedState(int actorId,State state)
    {
        //if (_stateChangedCallback == null) return;
        //_stateChangedCallback.Invoke(state);
        if (_stateChangedCallback.ContainsKey(actorId)) _stateChangedCallback[actorId]?.Invoke(state);
    }
    public void OnMoveInput(float x, float y)
    {
        if (_moveVelocityChangedCallback == null) return;
        _moveVelocityChangedCallback.Invoke(x,y);
    }

    public void OnActorRotate(float x, float y, float z)
    {
        if (_targetAngleChangedCallback == null) return;
        _targetAngleChangedCallback.Invoke(x, y, z);
    }

    public void OnHpChanged(int actorId,float damage)
    {
        if (_hpChangedCallbacks.ContainsKey(actorId)) _hpChangedCallbacks[actorId]?.Invoke(damage);
    }
    public void OnLockOnTarget(Transform target)
    {
        if(_lockOnTargetChangedCallback == null) return;
        _lockOnTargetChangedCallback.Invoke(target);
    }
    #endregion

    public bool OnChangedStateFalling(Transform target,float maxDistance, LayerMask groundLayer)
    {
        if (!IsGrounded(target, maxDistance, groundLayer))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private bool IsGrounded(Transform target ,float maxDistance, LayerMask groundLayer)
    {
        bool isGround = Physics.Raycast(target.position + new Vector3(0, maxDistance * 0.5f, 0), -target.up, maxDistance, groundLayer);
        return isGround;
    }
}
