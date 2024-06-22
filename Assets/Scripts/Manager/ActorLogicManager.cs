using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;

    private Dictionary<int, Action<State>> _stateChangedCallback = new Dictionary<int, Action<State>>();
    private Dictionary<int, Action<Monster_data>> _InfoChangedCallback = new Dictionary<int, Action<Monster_data>>();
    private Action<float, float> _moveVelocityChangedCallback;
    private Action<float, float, float> _targetAngleChangedCallback;
    private Dictionary<int, Action<float>> _hpChangedCallbacks = new Dictionary<int, Action<float>>();
    private Action<List<Transform>> _lockOnTargetListChangedCallback;
    private Action<Transform> _lockOnAbleTargetChangedCallback;
    private Action<Transform, InputViewModel> _lockOnTargetChangedCallback;
    private Action<Transform> _lockOnTargetChangeCallback;
    private Dictionary<int, Action<Transform>> _traceTargetChangedCallback = new Dictionary<int, Action<Transform>>();

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if(_instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
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

    public void RegisterInfoChangedCallback(int actorId, Action<Monster_data> infoChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_InfoChangedCallback.ContainsKey(actorId))
            {
                _InfoChangedCallback[actorId] = infoChangedCallback;
            }
            else
            {
                _InfoChangedCallback[actorId] += infoChangedCallback;
            }
        }
        else
        {
            if (_InfoChangedCallback.ContainsKey(actorId))
            {
                _InfoChangedCallback[actorId] -= infoChangedCallback;
                if (_InfoChangedCallback[actorId] == null) _InfoChangedCallback.Remove(actorId);
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

    public void RegisterLockOnTargetListChangedCallback(Action<List<Transform>> lockOnTargetListChangedCallback, bool isRegister)
    {
        if(isRegister) _lockOnTargetListChangedCallback += lockOnTargetListChangedCallback;
        else _lockOnTargetListChangedCallback -= lockOnTargetListChangedCallback;
    }

    public void RegisterLockOnTargetChangedCallback(Action<Transform, InputViewModel> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnTargetChangedCallback += lockOnTargetChangedCallback;
        else _lockOnTargetChangedCallback -= lockOnTargetChangedCallback;
    }

    public void RegisterLockOnTargetChangedCallback(Action<Transform> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnTargetChangeCallback += lockOnTargetChangedCallback;
        else _lockOnTargetChangeCallback -= lockOnTargetChangedCallback;
    }

    public void RegisterLockOnAbleTargetChangedCallback(Action<Transform> lockOnAbleTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnAbleTargetChangedCallback += lockOnAbleTargetChangedCallback;
        else _lockOnAbleTargetChangedCallback -= lockOnAbleTargetChangedCallback;
    }

    public void RegisterTraceTargetChangedCallback(Action<Transform> TraceTargetChangedCallback, int actorId, bool isRegister)
    {
        if(isRegister)
        {
            if (isRegister)
            {
                if (!_traceTargetChangedCallback.ContainsKey(actorId))
                {
                    _traceTargetChangedCallback[actorId] = TraceTargetChangedCallback;
                }
                else
                {
                    _traceTargetChangedCallback[actorId] += TraceTargetChangedCallback;
                }
            }
            else
            {
                if (_traceTargetChangedCallback.ContainsKey(actorId))
                {
                    _traceTargetChangedCallback[actorId] -= TraceTargetChangedCallback;
                    if (_traceTargetChangedCallback[actorId] == null) _traceTargetChangedCallback.Remove(actorId);
                }
            }
        }
    }
    #endregion

    #region Request 연결부
    public void OnChangedState(int actorId,State state)
    {
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

    public void OnInfoChanged(int actorId, Monster_data info)
    {
        if (_InfoChangedCallback.ContainsKey(actorId)) _InfoChangedCallback[actorId]?.Invoke(info);
    }

    public void OnHpChanged(int actorId,float damage)
    {
        if (_hpChangedCallbacks.ContainsKey(actorId)) _hpChangedCallbacks[actorId]?.Invoke(damage);
    }

    public void OnLockOnTargetList(List<Transform> lockOnTargetList)
    {
        if(_lockOnTargetListChangedCallback == null) return;
        _lockOnTargetListChangedCallback.Invoke(lockOnTargetList);
    }
    public void OnLockOnAbleTarget(Transform target)
    {
        if (_lockOnAbleTargetChangedCallback == null) return;
        _lockOnAbleTargetChangedCallback.Invoke(target);
    }
    public void OnLockOnTarget(Transform target, InputViewModel player)
    {
        if(_lockOnTargetChangedCallback == null) return;
        _lockOnTargetChangedCallback.Invoke(target, player);
    }

    public void OnLockOnTarget(Transform target)
    {
        if (_lockOnTargetChangedCallback == null) return;
        _lockOnTargetChangeCallback.Invoke(target);
    }
    public void OnTraceTarget(int actorId, Transform target)
    {
        if (_traceTargetChangedCallback.ContainsKey(actorId)) _traceTargetChangedCallback[actorId]?.Invoke(target);
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
