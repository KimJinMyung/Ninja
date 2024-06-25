using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Player_ViewModel
{
    private State _playerState;
    public State playerState
    {
        get { return _playerState; }
        set
        {
            if (_playerState == value) return;

            _playerState = value;
            OnPropertyChanged(nameof(playerState));
        }
    }

    private Vector2 _move;
    public Vector2 Move
    {
        get { return _move; }
        set
        {
            _move = value;
            OnPropertyChanged(nameof(Move));
        }
    }
    private Quaternion _rotation;
    public Quaternion Rotation
    {
        get { return _rotation; }
        set
        {
            _rotation = value;
            OnPropertyChanged(nameof(Rotation));
        }
    }
    private Transform _lockOnTarget;
    public Transform LockOnTarget
    {
        get { return _lockOnTarget; }
        set
        {
            if (_lockOnTarget == value) return;
            _lockOnTarget = value;
            OnPropertyChanged(nameof(LockOnTarget));
        }
    }

    #region propertyEvent

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion
}
