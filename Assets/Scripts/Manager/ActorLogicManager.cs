using System;
using UnityEngine;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if(_instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void RegisterMoveVelocityChangedCallback(Action<Vector2> moveVelocityCallback, bool isRegister)
    {

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
