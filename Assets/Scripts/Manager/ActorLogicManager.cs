using System;
using UnityEditor;
using UnityEngine;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;

    private Action<State> _stateChangedCallback;
    private Action<float, float, float> _transformPositionUpdateCallback;
    private Action<float, float> _moveVelocityChangedCallback;
    private Action<float, float, float> _targetAngleChangedCallback;
    private Action<bool> _isLockOnModeChangedCallback;
    private Action<int> _attackCountChangedCallback;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if(_instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    #region Register 연결부
    public void RegisterStateChangedCallback(Action<State> stateChangedCallback, bool isRegister)
    {
        if (isRegister) _stateChangedCallback += stateChangedCallback;
        else _stateChangedCallback -= stateChangedCallback;
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

    public void RegisterIsLockOnModeChangedCallback(Action<bool> isLockOnModeChangedCallback, bool isRegister)
    {
        if(isRegister) _isLockOnModeChangedCallback += isLockOnModeChangedCallback;
        else _isLockOnModeChangedCallback -= isLockOnModeChangedCallback;
    }

    #endregion

    #region Request 연결부
    public void OnChangedState(State state)
    {
        if (_stateChangedCallback == null) return;
        _stateChangedCallback.Invoke(state);
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

    public void OnIsLockOn(bool isLockOn)
    {
        if (_isLockOnModeChangedCallback == null) return;        
        _isLockOnModeChangedCallback.Invoke(isLockOn);
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
