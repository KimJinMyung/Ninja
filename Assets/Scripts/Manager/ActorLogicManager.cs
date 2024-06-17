using System;
using UnityEngine;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;

    private Action<float, float> _moveVelocityChangedCallback;
    private Action<float, float, float> _targetAngleChangedCallback;
    private Action<float, float> _mousePositionChangedCallback;

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
    public void RegisterMousePositionChangedCallback(Action<float, float> MousePositionChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            _mousePositionChangedCallback += MousePositionChangedCallback;
        }
        else
        {
            _mousePositionChangedCallback -= MousePositionChangedCallback;
        }
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
    public void OnMousePostition(float x, float y)
    {
        if (_mousePositionChangedCallback == null) return;

        _mousePositionChangedCallback.Invoke(x, y);
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
