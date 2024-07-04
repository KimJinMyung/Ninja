using System;
using System.Collections.Generic;
using UnityEngine;

public class ActorLogicManager : MonoBehaviour
{
    public static ActorLogicManager _instance = null;
    private Dictionary<int, Action<monsterType>> _monsterTypeChangedCallback = new Dictionary<int, Action<monsterType>>();
    private Dictionary<int, Action<State>> _stateChangedCallback = new Dictionary<int, Action<State>>();
    private Dictionary<int, Action<Player_data>> _playerDataChangedCallback = new Dictionary<int, Action<Player_data>>();
    private Dictionary<int, Action<Monster_data>> _InfoChangedCallback = new Dictionary<int, Action<Monster_data>>();
    private Action<float, float> _moveVelocityChangedCallback;
    private Action<float, float, float> _targetAngleChangedCallback;
    private Dictionary<int, Action<float>> _hpChangedCallbacks = new Dictionary<int, Action<float>>();
    private Dictionary<int, Action<Player_data>> _playerInfoChangedCallbacks = new Dictionary<int, Action<Player_data>>();
    private Action<List<Transform>> _lockOnTargetListChangedCallback;
    private Action<Transform> _lockOnAbleTargetChangedCallback;
    private Action<Transform, Player> _lockOnViewModel_TargetChangedCallback;
    private Action<Transform> _lockOnModelPlayer_TargetChangeCallback;
    private Action<AssassinatedType, Monster> _assassinatedTypeChangedCallback;
    private Dictionary<int, Action<Transform>> _traceTargetChangedCallback = new Dictionary<int, Action<Transform>>();
    private Dictionary<int, Action<List<Monster_Attack>, Monster>> _AttackMethodChangedCallback = new Dictionary<int, Action<List<Monster_Attack>, Monster>>();

    private void Awake()
    {
        if (_instance == null) _instance = this;
        else if(_instance != this) Destroy(this.gameObject);

        DontDestroyOnLoad(this.gameObject);
    }

    #region Register 연결부
    public void RegisterMonsterTypeChangedCallback(int actorId, Action<monsterType> monsterTypeChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_monsterTypeChangedCallback.ContainsKey(actorId))
            {
                _monsterTypeChangedCallback[actorId] = monsterTypeChangedCallback;
            }
            else
            {
                _monsterTypeChangedCallback.Add(actorId, monsterTypeChangedCallback);
            }
        }
        else
        {
            if (_monsterTypeChangedCallback.ContainsKey(actorId))
            {
                _monsterTypeChangedCallback[actorId] -= monsterTypeChangedCallback;
                if (_monsterTypeChangedCallback[actorId] == null) _stateChangedCallback.Remove(actorId);
            }
        }
    }

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
                _stateChangedCallback.Add(actorId, stateChangedCallback);
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

    public void RegisterPlayerDataChangedCallback(int actorId, Action<Player_data> playerDataChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_playerDataChangedCallback.ContainsKey(actorId))
            {
                _playerDataChangedCallback[actorId] = playerDataChangedCallback;
            }
            else
            {
                _playerDataChangedCallback.Add(actorId, playerDataChangedCallback);
            }
        }
        else
        {
            if (_playerDataChangedCallback.ContainsKey(actorId))
            {
                _playerDataChangedCallback[actorId] -= playerDataChangedCallback;
                if (_playerDataChangedCallback[actorId] == null) _playerDataChangedCallback.Remove(actorId);
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
                _InfoChangedCallback.Add(actorId, infoChangedCallback);
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
                _hpChangedCallbacks.Add(actorId, hpChangedCallback);
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
    public void RegisterPlayerInfoChangedCallback(int actorId, Action<Player_data> playerInfoChangedCallback, bool isRegister)
    {
        if (isRegister)
        {
            if (!_playerInfoChangedCallbacks.ContainsKey(actorId))
            {
                _playerInfoChangedCallbacks[actorId] = playerInfoChangedCallback;
            }
            else
            {
                _playerInfoChangedCallbacks.Add(actorId, playerInfoChangedCallback);
            }
        }
        else
        {
            if (_playerInfoChangedCallbacks.ContainsKey(actorId))
            {
                _playerInfoChangedCallbacks[actorId] -= playerInfoChangedCallback;
                if (_playerInfoChangedCallbacks[actorId] == null) _playerInfoChangedCallbacks.Remove(actorId);
            }
        }
    }

    public void RegisterLockOnTargetListChangedCallback(Action<List<Transform>> lockOnTargetListChangedCallback, bool isRegister)
    {
        if(isRegister) _lockOnTargetListChangedCallback += lockOnTargetListChangedCallback;
        else _lockOnTargetListChangedCallback -= lockOnTargetListChangedCallback;
    }

    public void RegisterLockOnViewModel_TargetChangedCallback(Action<Transform, Player> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnViewModel_TargetChangedCallback += lockOnTargetChangedCallback;
        else _lockOnViewModel_TargetChangedCallback -= lockOnTargetChangedCallback;
    }

    public void RegisterLockOnTargetChangedCallback(Action<Transform> lockOnTargetChangedCallback, bool isRegister)
    {
        if (isRegister) _lockOnModelPlayer_TargetChangeCallback += lockOnTargetChangedCallback;
        else _lockOnModelPlayer_TargetChangeCallback -= lockOnTargetChangedCallback;
    }
    public void RegisterAssassinatedChangedCallback(Action<AssassinatedType, Monster> assassinatedTypeChangedCallback, bool isRegister)
    {
        if (isRegister) _assassinatedTypeChangedCallback += assassinatedTypeChangedCallback;
        else _assassinatedTypeChangedCallback -= assassinatedTypeChangedCallback;
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
                    _traceTargetChangedCallback.Add(actorId, TraceTargetChangedCallback);
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

    public void RegisterAttackMethodChangedCallback(Action<List<Monster_Attack>, Monster> AttackMethodChangedCallback, int actorId, bool isRegister)
    {
        if (isRegister)
        {
            if (isRegister)
            {
                if (!_AttackMethodChangedCallback.ContainsKey(actorId))
                {
                    _AttackMethodChangedCallback[actorId] = AttackMethodChangedCallback;
                }
                else
                {
                    _AttackMethodChangedCallback.Add(actorId, AttackMethodChangedCallback);
                }
            }
            else
            {
                if (_AttackMethodChangedCallback.ContainsKey(actorId))
                {
                    _AttackMethodChangedCallback[actorId] -= AttackMethodChangedCallback;
                    if (_AttackMethodChangedCallback[actorId] == null) _AttackMethodChangedCallback.Remove(actorId);
                }
            }
        }
    }
    #endregion

    #region Request 연결부
    public void OnChangedMonsterType(int actorId, monsterType type)
    {
        if (_monsterTypeChangedCallback.ContainsKey(actorId)) _monsterTypeChangedCallback[actorId]?.Invoke(type);
    }
    public void OnChangedState(int actorId,State state)
    {
        if (_stateChangedCallback.ContainsKey(actorId)) _stateChangedCallback[actorId]?.Invoke(state);
    }
    public void OnChangedPlayerData(int actorId, Player_data state)
    {
        if (_playerDataChangedCallback.ContainsKey(actorId)) _playerDataChangedCallback[actorId]?.Invoke(state);
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
    public void OnPlayerInfoChanged(int actorId, Player_data data)
    {
        if (_playerInfoChangedCallbacks.ContainsKey(actorId)) _playerInfoChangedCallbacks[actorId]?.Invoke(data);
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
    public void OnLockOnTarget_LockOnViewModel(Transform target, Player player)
    {
        if(_lockOnViewModel_TargetChangedCallback == null) return;
        _lockOnViewModel_TargetChangedCallback.Invoke(target, player);
    }

    public void OnLockOnTarget(Transform target)
    {
        if (_lockOnModelPlayer_TargetChangeCallback == null) return;
        _lockOnModelPlayer_TargetChangeCallback.Invoke(target);
    }
    public void OnAssassinated(AssassinatedType type, Monster monster)
    {
        if (_assassinatedTypeChangedCallback == null) return;
        _assassinatedTypeChangedCallback.Invoke(type, monster);
    }
    public void OnTraceTarget(int actorId, Transform target)
    {
        if (_traceTargetChangedCallback.ContainsKey(actorId)) _traceTargetChangedCallback[actorId]?.Invoke(target);
    }
    public void OnAttackMethodChanged(int actorId, List<Monster_Attack> attackList, Monster owner)
    {
        if (_AttackMethodChangedCallback.ContainsKey(actorId)) _AttackMethodChangedCallback[actorId]?.Invoke(attackList, owner);
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
