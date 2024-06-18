using Cinemachine;
using System.ComponentModel;
using UnityEngine;

public class InputViewModel
{
    private State _playerState;
    public State PlayerState
    {
        get { return _playerState; }
        set
        {
            if (_playerState == value) return;

            _playerState = value;
            OnPropertyChanged(nameof(PlayerState));
            //_stateMachine.ChangeState(_playerState);
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

    private bool _isLockOnMode;
    public bool IsLockOnMode
    {
        get { return _isLockOnMode; }
        set
        {
            _isLockOnMode = value;
            OnPropertyChanged(nameof(IsLockOnMode));
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
