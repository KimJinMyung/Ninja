using System.ComponentModel;
using UnityEngine;

public class InputViewModel
{
    private Player_data _player_Data;
    public Player_data player_Data
    {
        get { return _player_Data; }
        set
        {
            if(_player_Data == value) return;

            _player_Data = value;
            OnPropertyChanged(nameof(player_Data));
        }
    }

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

    private float _maxHp;
    public float MaxHp
    {
        get => _maxHp;
        set
        {
            if (_maxHp == value) return;
            _maxHp = value;
            OnPropertyChanged(nameof(MaxHp));
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
