using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;

    private Action<float, float> _moveVelocityChangedCallback;
    private Action<float, float> _targetAngleChangedCallback;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if(_instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
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

    public void RegisterTargetAngleChangedCallback(Action<float, float> targetAngleChangedCallback, bool isRegister)
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

    public void OnMoveInput(float x, float y)
    {
        if (_moveVelocityChangedCallback == null) return;

        _moveVelocityChangedCallback.Invoke(x,y);
    }

    public void OnMoveDir(float x, float y)
    {
        if (_targetAngleChangedCallback == null) return;

        _targetAngleChangedCallback.Invoke(x, y);
    }

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
